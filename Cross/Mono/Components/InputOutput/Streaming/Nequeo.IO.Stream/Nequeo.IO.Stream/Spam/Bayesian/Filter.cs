/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

using Nequeo.IO.Stream.Spam;

namespace Nequeo.IO.Stream.Spam.Bayesian
{
    /// <summary>
    /// Native Bayesian Spam Filter
    /// </summary>
    public class Filter
    {
        #region Knobs for dialing in performance
        /// <summary>
        /// These are constants used in the Bayesian algorithm, presented in a form that lets you monkey with them.
        /// </summary>
        public class KnobList
        {
            /// <summary>
            /// Good Token Weight
            /// </summary>
            public int GoodTokenWeight = 2;				// 2
            /// <summary>
            /// Min Token Count
            /// </summary>
            public int MinTokenCount = 0;				// 0
            /// <summary>
            /// Min Count For Inclusion
            /// </summary>
            public int MinCountForInclusion = 5;		// 5
            /// <summary>
            /// Min Score
            /// </summary>
            public double MinScore = 0.011;				// 0.01
            /// <summary>
            /// Max Score
            /// </summary>
            public double MaxScore = 0.99;				// 0.99
            /// <summary>
            /// Likely Spam Score
            /// </summary>
            public double LikelySpamScore = 0.9998;		// 0.9998
            /// <summary>
            /// Certain Spam Score
            /// </summary>
            public double CertainSpamScore = 0.9999;	// 0.9999
            /// <summary>
            /// Certain Spam Count
            /// </summary>
            public int CertainSpamCount = 10;			// 10
            /// <summary>
            /// Interesting Word Count
            /// </summary>
            public int InterestingWordCount = 15;		// 15 (later changed to 20)
        }

        private KnobList _knobs = new KnobList();

        /// <summary>
        /// These are the knobs you can turn to dial in performance on the algorithm.
        /// Hopefully the names make a little bit of sense and you can find where
        /// they fit into the original algorithm.
        /// </summary>
        public KnobList Knobs
        {
            get { return _knobs; }
            set { _knobs = value; }
        }

        #endregion

        #region Private Fields
        private TextControlList _good;
        private TextControlList _bad;
        private SortedDictionary<string, double> _prob;
        private int _ngood;
        private int _nbad;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, a list of words that show up in Spam text
        /// </summary>
        public TextControlList Bad
        {
            get { return _bad; }
            set { _bad = value; }
        }

        /// <summary>
        /// Gets sets, a list of words that show up in non-spam text
        /// </summary>
        public TextControlList Good
        {
            get { return _good; }
            set { _good = value; }
        }

        /// <summary>
        /// Gets sets, a list of probabilities that the given word might appear in a Spam text
        /// </summary>
        public SortedDictionary<string, double> Prob
        {
            get { return _prob; }
            set { _prob = value; }
        }
        #endregion

        #region population

        /// <summary>
        /// Initialize the SpamFilter based on the supplied text
        /// </summary>
        /// <param name="goodReader">File containing non-spam</param>
        /// <param name="badReader">File containing spam</param>
        public void Load(TextReader goodReader, TextReader badReader)
        {
            _good = new TextControlList(goodReader);
            _bad = new TextControlList(badReader);

            CalculateProbabilities();
        }

        /// <summary>
        /// Initialize the SpamFilter based on the contents of the supplied text control list.
        /// </summary>
        /// <param name="good">Text control list containing non-spam</param>
        /// <param name="bad">Text control list  containing spam</param>
        public void Load(TextControlList good, TextControlList bad)
        {
            _good = good;
            _bad = bad;

            CalculateProbabilities();
        }

        /// <summary>
        /// Initialize the SpamFilter based on a DataTable containing columns "IsSpam" and "Body".
        /// </summary>
        /// <param name="table">The data table containing the data.</param>
        public void Load(DataTable table)
        {
            _good = new TextControlList();
            _bad = new TextControlList();

            foreach (DataRow row in table.Rows)
            {
                bool isSpam = (bool)row["IsSpam"];
                string body = row["Body"].ToString();
                if (isSpam)
                {
                    _bad.LoadFromReader(new StringReader(body));
                }
                else
                {
                    _good.LoadFromReader(new StringReader(body));
                }
            }

            CalculateProbabilities();
        }

        /// <summary>
        /// Do the math to populate the probabilities collection
        /// </summary>
        private void CalculateProbabilities()
        {
            _prob = new SortedDictionary<string, double>();

            _ngood = _good.Tokens.Count;
            _nbad = _bad.Tokens.Count;
            foreach (string token in _good.Tokens.Keys)
            {
                CalculateTokenProbability(token);
            }
            foreach (string token in _bad.Tokens.Keys)
            {
                if (!_prob.ContainsKey(token))
                {
                    CalculateTokenProbability(token);
                }
            }
        }

        /// <summary>
        /// For a given token, calculate the probability that will appear in a spam text
        /// by comparing the number of good and bad texts it appears in already.
        /// </summary>
        /// <param name="token">The token (word) to calculate the probability for.</param>
        private void CalculateTokenProbability(string token)
        {
            /*
             * This is a direct implementation of Paul Graham's algorithm from
             * http://www.paulgraham.com/spam.html
             * 
             *	(let ((g (* 2 (or (gethash word good) 0)))
             *		  (b (or (gethash word bad) 0)))
             *	   (unless (< (+ g b) 5)
             *		 (max .01
             *			  (min .99 (float (/ (min 1 (/ b nbad))
             *								 (+ (min 1 (/ g ngood))   
             *									(min 1 (/ b nbad)))))))))
             */

            int g = _good.Tokens.ContainsKey(token) ? _good.Tokens[token] * Knobs.GoodTokenWeight : 0;
            int b = _bad.Tokens.ContainsKey(token) ? _bad.Tokens[token] : 0;

            if (g + b >= Knobs.MinCountForInclusion)
            {
                double goodfactor = Min(1, (double)g / (double)_ngood);
                double badfactor = Min(1, (double)b / (double)_nbad);

                double prob = Max(Knobs.MinScore,
                                Min(Knobs.MaxScore, badfactor / (goodfactor + badfactor))
                            );

                // special case for Spam-only tokens.
                // .9998 for tokens only found in spam, or .9999 if found more than 10 times
                if (g == 0)
                {
                    prob = (b > Knobs.CertainSpamCount) ? Knobs.CertainSpamScore : Knobs.LikelySpamScore;
                }

                _prob[token] = prob;
            }
        }
        #endregion

        #region Public Serialization
        /// <summary>
        /// Dumps the probability list to a file, preceded by a line containing good, bad and probability counts.
        /// </summary>
        /// <param name="filePath">The file path to write to.</param>
        public void ToFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter writer = new StreamWriter(fs);

                writer.WriteLine(String.Format("{0},{1},{2}", _ngood, _nbad, _prob.Count));
                foreach (string key in _prob.Keys)
                {
                    writer.WriteLine(String.Format("{0},{1}", _prob[key].ToString("#.#####"), key));
                }

                writer.Flush();
                fs.Close();
            }
        }

        /// <summary>
        /// Populate from a file created with ToFile().
        /// </summary>
        /// <param name="filePath">The file path to read from.</param>
        public void FromFile(string filePath)
        {
            _prob = new SortedDictionary<string, double>();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                StreamReader reader = new StreamReader(fs);

                ParseCounts(reader.ReadLine());

                while (!reader.EndOfStream)
                {
                    ParseProb(reader.ReadLine());
                }

                fs.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        private void ParseCounts(string line)
        {
            string[] tokens = line.Split(',');
            if (tokens.Length > 1)
            {
                _ngood = Convert.ToInt32(tokens[0]);
                _nbad = Convert.ToInt32(tokens[1]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        private void ParseProb(string line)
        {
            string[] tokens = line.Split(',');
            if (tokens.Length > 1)
            {
                _prob.Add(tokens[1], Convert.ToDouble(tokens[0]));
            }
        }

        #endregion

        #region Public Spam Testing
        /// <summary>
        /// Returns the probability that the supplied body of text is spam
        /// </summary>
        /// <param name="body">The text to test.</param>
        /// <returns>The probability.</returns>
        public double Test(string body)
        {
            SortedList probs = new SortedList();

            // Spin through every word in the body and look up its individual spam probability.
            // Keep the list in decending order of "Interestingness"
            Regex re = new Regex(TextControlList.TokenPattern, RegexOptions.Compiled);
            Match m = re.Match(body);
            int index = 0;
            while (m.Success)
            {
                string token = m.Groups[1].Value;
                if (_prob.ContainsKey(token))
                {
                    // "interestingness" == how far our score is from 50%.  
                    // The crazy math below is building a string that lets us sort alphabetically by interestingness.
                    double prob = _prob[token];
                    string key = (0.5 - System.Math.Abs(0.5 - prob)).ToString(".00000") + token + index++;
                    probs.Add(key, prob);

                }

                m = m.NextMatch();
            }

            /* Combine the 15 most interesting probabilities together into one.  
             * The algorithm to do this is shown below and described here:
             * http://www.paulgraham.com/naivebayes.html
             * 
             *				abc           
             *	---------------------------
             *	abc + (1 - a)(1 - b)(1 - c)
             *
             */

            double mult = 1;  // for holding abc..n
            double comb = 1;  // for holding (1 - a)(1 - b)(1 - c)..(1-n)
            index = 0;
            foreach (string key in probs.Keys)
            {
                double prob = (double)probs[key];
                mult = mult * prob;
                comb = comb * (1 - prob);

                if (++index > Knobs.InterestingWordCount)
                    break;
            }

            return mult / (mult + comb);
        }
        #endregion

        #region Private helpers
        /// <summary>
        /// Get the maximun of two numbers.
        /// </summary>
        /// <param name="one">First naumber</param>
        /// <param name="two">Second number</param>
        /// <returns>The maximum</returns>
        private double Max(double one, double two)
        {
            return one > two ? one : two;
        }

        /// <summary>
        /// Get the minimum of two numbers.
        /// </summary>
        /// <param name="one">First naumber</param>
        /// <param name="two">Second number</param>
        /// <returns>The mimimum</returns>
        private double Min(double one, double two)
        {
            return one < two ? one : two;
        }
        #endregion
    }
}
