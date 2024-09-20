namespace TranslationProjectManagement.Utilities
{
    /// <summary>
    /// Provides methods for generating sequentially ordered GUIDs (Globally Unique Identifiers).
    /// These GUIDs are designed to be sortable based on their creation time, which improves database indexing performance.
    /// </summary>
    public static class SequentialGuid
    {
        // The offset used to adjust GUID timestamps to a compatible format.
        private static readonly long guidEpochOffset = -5748192000000000; // Offset for GUID timestamps

        // The last tick value used to ensure sequential generation of GUIDs.
        private static long _lastTick = DateTime.UtcNow.Ticks + guidEpochOffset;

        /// <summary>
        /// Generates a new sequential GUID that is ordered chronologically based on its creation time.
        /// This method ensures that each GUID generated is unique and sequential, which helps to reduce fragmentation
        /// and improve performance in databases.
        /// </summary>
        /// <returns>A new sequentially ordered GUID.</returns>
        public static Guid NewSequentialGuid()
        {
            // Create a new GUID and convert it to a byte array.
            byte[] guidBytes = Guid.NewGuid().ToByteArray();

            // Get the current time in ticks and apply the epoch offset.
            long currentTicks = DateTime.UtcNow.Ticks + guidEpochOffset;

            // Variable to hold the last tick value for comparison.
            long last;

            // Ensure the current tick value is unique and greater than the last tick value.
            do
            {
                last = Interlocked.Read(ref _lastTick);
                if (currentTicks <= last)
                    currentTicks = last + 1;
            }
            while (Interlocked.CompareExchange(ref _lastTick, currentTicks, last) != last);

            // Convert the current ticks to a byte array for embedding in the GUID.
            byte[] counterBytes = BitConverter.GetBytes(currentTicks);

            // Ensure the byte array is in little-endian format for consistency.
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(counterBytes);
            }

            // Embed the tick bytes into specific positions in the GUID byte array to ensure chronological ordering.
            guidBytes[8] = counterBytes[1];
            guidBytes[9] = counterBytes[0];
            guidBytes[10] = counterBytes[7];
            guidBytes[11] = counterBytes[6];
            guidBytes[12] = counterBytes[5];
            guidBytes[13] = counterBytes[4];
            guidBytes[14] = counterBytes[3];
            guidBytes[15] = counterBytes[2];

            return new Guid(guidBytes);
        }
    }
}
