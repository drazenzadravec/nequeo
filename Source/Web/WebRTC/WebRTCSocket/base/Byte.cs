using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;

namespace WebRTCSocket
{
	/// <summary>
	/// Class that extends the Byte[] type.
	/// </summary>
	public static class ByteExtensions
	{
		/// <summary>
		/// Combine the array to the source array, append to the end of the array
		/// </summary>
		/// <param name="source">The source array.</param>
		/// <param name="arrayOne">The first array to combine.</param>
		/// <returns>The new byte array.</returns>
		/// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
		public static Byte[] Combine(this Byte[] source, Byte[] arrayOne)
		{
			// If the source object is null.
			if (source == null) throw new System.ArgumentNullException("source");
			if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");

			// Set the total array size.
			Byte[] data = new byte[source.Length + arrayOne.Length];

			// Execute the async concurrent tasks.
			Task.Factory.ContinueWhenAll(new Task[]
				 {
						  SourceArray(data, source, 0),
						  SourceArray(data, arrayOne, (source.Length))
				 }, completedTasks =>
				 {
						 // Do nothing
						 int failures = completedTasks.Where(t => t.Exception != null).Count();
				 }).Wait();

			// Return the new array.
			return data;
		}

		/// <summary>
		/// Combine the array in parallel to the source array, append to the end of the array
		/// </summary>
		/// <param name="source">The source array.</param>
		/// <param name="arrayOne">The first array to combine.</param>
		/// <returns>The new byte array.</returns>
		/// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
		public static Byte[] CombineParallel(this Byte[] source, Byte[] arrayOne)
		{
			// If the source object is null.
			if (source == null) throw new System.ArgumentNullException("source");
			if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");

			// Set the total array size.
			Byte[] data = new byte[source.Length + arrayOne.Length];

			// Execute the async concurrent tasks.
			Parallel.For(0, 2, j =>
			{
				switch (j)
				{
					case 0:
						SourceArrayParallel(data, source, 0);
						break;
					case 1:
						SourceArrayParallel(data, arrayOne, (source.Length));
						break;
				}
			});

			// Return the new array.
			return data;
		}

		/// <summary>
		/// Combine the array to the source array, append to the end of the array
		/// </summary>
		/// <param name="source">The source array.</param>
		/// <param name="arrayOne">The first array to combine.</param>
		/// <param name="arrayTwo">The second array to combine.</param>
		/// <returns>The new byte array.</returns>
		/// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
		public static Byte[] Combine(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo)
		{
			// If the source object is null.
			if (source == null) throw new System.ArgumentNullException("source");
			if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
			if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");

			// Set the total array size.
			Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length];

			// Execute the async concurrent tasks.
			Task.Factory.ContinueWhenAll(new Task[]
				 {
						  SourceArray(data, source, 0),
						  SourceArray(data, arrayOne, (source.Length)),
						  SourceArray(data, arrayTwo, (source.Length + arrayOne.Length)),
				 }, completedTasks =>
				 {
						 // Do nothing
						 int failures = completedTasks.Where(t => t.Exception != null).Count();
				 }).Wait();

			// Return the new array.
			return data;
		}

		/// <summary>
		/// Combine the array in parallel to the source array, append to the end of the array
		/// </summary>
		/// <param name="source">The source array.</param>
		/// <param name="arrayOne">The first array to combine.</param>
		/// <param name="arrayTwo">The second array to combine.</param>
		/// <returns>The new byte array.</returns>
		/// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
		public static Byte[] CombineParallel(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo)
		{
			// If the source object is null.
			if (source == null) throw new System.ArgumentNullException("source");
			if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
			if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");

			// Set the total array size.
			Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length];

			// Execute the async concurrent tasks.
			Parallel.For(0, 3, j =>
			{
				switch (j)
				{
					case 0:
						SourceArrayParallel(data, source, 0);
						break;
					case 1:
						SourceArrayParallel(data, arrayOne, (source.Length));
						break;
					case 2:
						SourceArrayParallel(data, arrayTwo, (source.Length + arrayOne.Length));
						break;
				}
			});

			// Return the new array.
			return data;
		}

		/// <summary>
		/// Combine the array to the source array, append to the end of the array
		/// </summary>
		/// <param name="source">The source array.</param>
		/// <param name="arrayOne">The first array to combine.</param>
		/// <param name="arrayTwo">The second array to combine.</param>
		/// <param name="arrayThree">The third array to combine.</param>
		/// <returns>The new byte array.</returns>
		/// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
		public static Byte[] Combine(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree)
		{
			// If the source object is null.
			if (source == null) throw new System.ArgumentNullException("source");
			if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
			if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
			if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");

			// Set the total array size.
			Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length];

			// Execute the async concurrent tasks.
			Task.Factory.ContinueWhenAll(new Task[]
				 {
						  SourceArray(data, source, 0),
						  SourceArray(data, arrayOne, (source.Length)),
						  SourceArray(data, arrayTwo, (source.Length + arrayOne.Length)),
						  SourceArray(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length))
				 }, completedTasks =>
				 {
						 // Do nothing
						 int failures = completedTasks.Where(t => t.Exception != null).Count();
				 }).Wait();

			// Return the new array.
			return data;
		}

		/// <summary>
		/// Combine the array in parallel to the source array, append to the end of the array
		/// </summary>
		/// <param name="source">The source array.</param>
		/// <param name="arrayOne">The first array to combine.</param>
		/// <param name="arrayTwo">The second array to combine.</param>
		/// <param name="arrayThree">The third array to combine.</param>
		/// <returns>The new byte array.</returns>
		/// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
		public static Byte[] CombineParallel(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree)
		{
			// If the source object is null.
			if (source == null) throw new System.ArgumentNullException("source");
			if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
			if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
			if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");

			// Set the total array size.
			Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length];

			// Execute the async concurrent tasks.
			Parallel.For(0, 4, j =>
			{
				switch (j)
				{
					case 0:
						SourceArrayParallel(data, source, 0);
						break;
					case 1:
						SourceArrayParallel(data, arrayOne, (source.Length));
						break;
					case 2:
						SourceArrayParallel(data, arrayTwo, (source.Length + arrayOne.Length));
						break;
					case 3:
						SourceArrayParallel(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length));
						break;
				}
			});

			// Return the new array.
			return data;
		}

		/// <summary>
		/// Combine the array to the source array, append to the end of the array
		/// </summary>
		/// <param name="source">The source array.</param>
		/// <param name="arrayOne">The first array to combine.</param>
		/// <param name="arrayTwo">The second array to combine.</param>
		/// <param name="arrayThree">The third array to combine.</param>
		/// <param name="arrayFour">The fourth array to combine.</param>
		/// <returns>The new byte array.</returns>
		/// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
		public static Byte[] Combine(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree, Byte[] arrayFour)
		{
			// If the source object is null.
			if (source == null) throw new System.ArgumentNullException("source");
			if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
			if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
			if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");
			if (arrayFour == null) throw new System.ArgumentNullException("arrayFour");

			// Set the total array size.
			Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length + arrayFour.Length];

			// Execute the async concurrent tasks.
			Task.Factory.ContinueWhenAll(new Task[]
				 {
						  SourceArray(data, source, 0),
						  SourceArray(data, arrayOne, (source.Length)),
						  SourceArray(data, arrayTwo, (source.Length + arrayOne.Length)),
						  SourceArray(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length)),
						  SourceArray(data, arrayFour, (source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length))
				 }, completedTasks =>
				 {
						 // Do nothing
						 int failures = completedTasks.Where(t => t.Exception != null).Count();
				 }).Wait();

			// Return the new array.
			return data;
		}

		/// <summary>
		/// Combine the array in parallel to the source array, append to the end of the array
		/// </summary>
		/// <param name="source">The source array.</param>
		/// <param name="arrayOne">The first array to combine.</param>
		/// <param name="arrayTwo">The second array to combine.</param>
		/// <param name="arrayThree">The third array to combine.</param>
		/// <param name="arrayFour">The fourth array to combine.</param>
		/// <returns>The new byte array.</returns>
		/// <exception cref="System.ArgumentNullException">Source object can not be null.</exception>
		public static Byte[] CombineParallel(this Byte[] source, Byte[] arrayOne, Byte[] arrayTwo, Byte[] arrayThree, Byte[] arrayFour)
		{
			// If the source object is null.
			if (source == null) throw new System.ArgumentNullException("source");
			if (arrayOne == null) throw new System.ArgumentNullException("arrayOne");
			if (arrayTwo == null) throw new System.ArgumentNullException("arrayTwo");
			if (arrayThree == null) throw new System.ArgumentNullException("arrayThree");
			if (arrayFour == null) throw new System.ArgumentNullException("arrayFour");

			// Set the total array size.
			Byte[] data = new byte[source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length + arrayFour.Length];

			// Execute the async concurrent tasks.
			Parallel.For(0, 5, j =>
			{
				switch (j)
				{
					case 0:
						SourceArrayParallel(data, source, 0);
						break;
					case 1:
						SourceArrayParallel(data, arrayOne, (source.Length));
						break;
					case 2:
						SourceArrayParallel(data, arrayTwo, (source.Length + arrayOne.Length));
						break;
					case 3:
						SourceArrayParallel(data, arrayThree, (source.Length + arrayOne.Length + arrayTwo.Length));
						break;
					case 4:
						SourceArrayParallel(data, arrayFour, (source.Length + arrayOne.Length + arrayTwo.Length + arrayThree.Length));
						break;
				}
			});

			// Return the new array.
			return data;
		}

		/// <summary>
		/// Apply the task for the byte array.
		/// </summary>
		/// <param name="result">The result array.</param>
		/// <param name="source">The source array.</param>
		/// <param name="start">The starting index of the result array.</param>
		/// <returns>The resulting byte array.</returns>
		internal static Task<byte[]> SourceArray(Byte[] result, Byte[] source, int start)
		{
			// Create the task to be returned
			var tcs = new TaskCompletionSource<byte[]>(source);

			// Assign the source array.
			for (int i = 0; i < source.Length; i++)
				result[i + start] = source[i];

			// Set the completion result.
			// this indicates that this task
			// has run to completion.
			tcs.SetResult(result);

			// Return the task that represents the async operation
			return tcs.Task;
		}

		/// <summary>
		/// Apply the task for the byte array.
		/// </summary>
		/// <param name="result">The result array.</param>
		/// <param name="source">The source array.</param>
		/// <param name="start">The starting index of the result array.</param>
		/// <returns>The resulting byte array.</returns>
		internal static void SourceArrayParallel(Byte[] result, Byte[] source, int start)
		{
			// Assign the source array.
			for (int i = 0; i < source.Length; i++)
				result[i + start] = source[i];
		}
	}
}