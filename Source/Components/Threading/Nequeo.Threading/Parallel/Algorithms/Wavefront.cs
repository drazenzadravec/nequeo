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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nequeo.Threading.Parallel.Algorithms
{
    public static partial class ParallelAlgorithms
    {
        /// <summary>Process in parallel a matrix where every cell has a dependency on the cell above it and to its left.</summary>
        /// <param name="numRows">The number of rows in the matrix.</param>
        /// <param name="numColumns">The number of columns in the matrix.</param>
        /// <param name="numBlocksPerRow">Partition the matrix into this number of blocks along the rows.</param>
        /// <param name="numBlocksPerColumn">Partition the matrix into this number of blocks along the columns.</param>
        /// <param name="processBlock">The action to invoke for every block, supplied with the start and end indices of the rows and columns.</param>
        public static void Wavefront(
            int numRows, int numColumns,
            int numBlocksPerRow, int numBlocksPerColumn,
            Action<int, int, int, int> processBlock)
        {
            // Validate parameters
            if (numRows <= 0) throw new ArgumentOutOfRangeException("numRows");
            if (numColumns <= 0) throw new ArgumentOutOfRangeException("numColumns");
            if (numBlocksPerRow <= 0 || numBlocksPerRow > numRows)
                throw new ArgumentOutOfRangeException("numBlocksPerRow");
            if (numBlocksPerColumn <= 0 || numBlocksPerColumn > numColumns)
                throw new ArgumentOutOfRangeException("numBlocksPerColumn");
            if (processBlock == null)
                throw new ArgumentNullException("processRowColumnCell");

            // Compute the size of each block
            int rowBlockSize = numRows / numBlocksPerRow;
            int columnBlockSize = numColumns / numBlocksPerColumn;

            Wavefront(numBlocksPerRow, numBlocksPerColumn, (row, column) =>
            {
                int start_i = row * rowBlockSize;
                int end_i = row < numBlocksPerRow - 1 ?
                    start_i + rowBlockSize : numRows;

                int start_j = column * columnBlockSize;
                int end_j = column < numBlocksPerColumn - 1 ?
                    start_j + columnBlockSize : numColumns;

                processBlock(start_i, end_i, start_j, end_j);
            });
        }

        /// <summary>Process in parallel a matrix where every cell has a dependency on the cell above it and to its left.</summary>
        /// <param name="numRows">The number of rows in the matrix.</param>
        /// <param name="numColumns">The number of columns in the matrix.</param>
        /// <param name="processRowColumnCell">The action to invoke for every cell, supplied with the row and column indices.</param>
        public static void Wavefront(int numRows, int numColumns, Action<int, int> processRowColumnCell)
        {
            // Validate parameters
            if (numRows <= 0) throw new ArgumentOutOfRangeException("numRows");
            if (numColumns <= 0) throw new ArgumentOutOfRangeException("numColumns");
            if (processRowColumnCell == null) throw new ArgumentNullException("processRowColumnCell");

            // Store the previous row of tasks as well as the previous task in the current row
            Task[] prevTaskRow = new Task[numColumns];
            Task prevTaskInCurrentRow = null;
            var dependencies = new Task[2];

            // Create a task for each cell
            for (int row = 0; row < numRows; row++)
            {
                prevTaskInCurrentRow = null;
                for (int column = 0; column < numColumns; column++)
                {
                    // In-scope locals for being captured in the task closures
                    int j = row, i = column;

                    // Create a task with the appropriate dependencies.
                    Task curTask;
                    if (row == 0 && column == 0)
                    {
                        // Upper-left task kicks everything off, having no dependencies
                        curTask = Task.Factory.StartNew(() => processRowColumnCell(j, i));
                    }
                    else if (row == 0 || column == 0)
                    {
                        // Tasks in the left-most column depend only on the task above them, and
                        // tasks in the top row depend only on the task to their left
                        var antecedent = column == 0 ? prevTaskRow[0] : prevTaskInCurrentRow;
                        curTask = antecedent.ContinueWith(p =>
                        {
                            p.Wait(); // Necessary only to propagate exceptions
                            processRowColumnCell(j, i);
                        });
                    }
                    else // row > 0 && column > 0
                    {
                        // All other tasks depend on both the tasks above and to the left
                        dependencies[0] = prevTaskInCurrentRow;
                        dependencies[1] = prevTaskRow[column];
                        curTask = Task.Factory.ContinueWhenAll(dependencies, ps =>
                        {
                            Task.WaitAll(ps); // Necessary only to propagate exceptions
                            processRowColumnCell(j, i);
                        });
                    }

                    // Keep track of the task just created for future iterations
                    prevTaskRow[column] = prevTaskInCurrentRow = curTask;
                }
            }

            // Wait for the last task to be done.
            prevTaskInCurrentRow.Wait();
        }
    }
}