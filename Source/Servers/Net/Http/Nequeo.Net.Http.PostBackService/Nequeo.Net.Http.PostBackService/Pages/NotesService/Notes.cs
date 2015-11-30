/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using Nequeo.Handler;
using Nequeo.ComponentModel.Composition;
using Nequeo.Net.Http.PostBackService.Common;
using Nequeo.Net.Http.Common;

namespace Nequeo.Net.Http.PostBackService.Pages.NotesService
{
    /// <summary>
    /// Notes page implementation.
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    internal class Notes : HttpPageBase
    {
        /// <summary>
        /// The page is initialised.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public override void OnInit(HttpPageContext pageContext)
        {
        }

        /// <summary>
        /// The page is loading.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public override void OnLoad(HttpPageContext pageContext)
        {
            try
            {
                string notesHtmlData = "";

                // Get the response data.
                Data.NotesFormService.notes noteHtml = new Data.NotesFormService.notes();

                // If post back
                if (base.IsPostBack)
                {
                    // If form data exists then
                    // wrire ti the file.
                    if (base.Form != null)
                    {
                        if (base.Form.Count > 0)
                        {
                            StreamWriter streamWriter = null;
                            try
                            {
                                // Get the 'NoteName' key name from the form.
                                string notename = base.Form.AllKeys.Where(u => u.ToLower() == "notename".ToLower()).First();

                                // Save path.
                                string savePath = base.UploadDirectory + "Notes\\" + base.Form[notename] + ".txt";

                                // If the directory does not exist
                                // then create the directory.
                                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                                // Create a new file or over write extisting file.
                                using (streamWriter = System.IO.File.CreateText(savePath))
                                {
                                    for (int i = 0; i < base.Form.Count; i++)
                                    {
                                        // Write the line of data.
                                        streamWriter.WriteLine(base.Form.Keys[i] + " : " + base.Form.Get(i));
                                    }

                                    // Close the stream.
                                    streamWriter.Flush();
                                    streamWriter.Close();
                                }
                            }
                            catch (Exception fex)
                            {
                                LogHandler.WriteTypeMessage(
                                    fex.Message,
                                    MethodInfo.GetCurrentMethod(),
                                    Helper.EventApplicationName());
                            }
                            finally
                            {
                                if (streamWriter != null)
                                    streamWriter.Close();
                            }

                            // Get the response data.
                            notesHtmlData = noteHtml.TransformText().
                                Replace("value=\"NoteName\"", "value=\"" + base.Form.Get("NoteName") + "\"").
                                Replace("value=\"01/01/01\"", "value=\"" + base.Form.Get("NoteDate") + "\"").
                                Replace("value=\"ContextName\"", "value=\"" + base.Form.Get("ContextName") + "\"").
                                Replace("value=\"NoteData\"", base.Form.Get("NoteData"));
                        }
                    }
                }
                else
                {
                    // Get the response data.
                    notesHtmlData = noteHtml.TransformText().
                        Replace("value=\"NoteName\"", "").
                        Replace("value=\"01/01/01\"", "").
                        Replace("value=\"ContextName\"", "").
                        Replace("value=\"NoteData\"", "");
                }
                
                // Send the new content.
                base.AlternativeContent = Encoding.UTF8.GetBytes(notesHtmlData);
            }
            catch (Exception ex)
            {
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Helper.EventApplicationName());
            }
        }

        /// <summary>
        /// The pre-process event.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public override void OnPreProcess(HttpPageContext pageContext)
        {
            // Get the download file directory.
            base.UploadDirectory = Helper.BaseDownloadPath().TrimEnd('\\') + "\\";
        }
    }
}
