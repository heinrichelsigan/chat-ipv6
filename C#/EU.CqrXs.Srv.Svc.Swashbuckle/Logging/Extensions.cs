namespace EU.CqrXs.Srv.SvcSwashbuckle.Logging
{
    public static class Extensions
    {
        #region DateTime extensions

        /// <summary>
        /// Area23Date extension method for DateTime
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/></param>
        /// <returns>formatted date <see cref="string"/></returns>
        public static string Enabler4BizDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Area23DateTime extension method for DateTime
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/></param>
        /// <returns>formatted date time <see cref="string"/> </returns>
        public static string Enabler4BizDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy") + Constants.DATE_DELIM +
                DateTime.UtcNow.ToString("MM") + Constants.DATE_DELIM +
                DateTime.UtcNow.ToString("dd") + Constants.WHITE_SPACE +
                DateTime.UtcNow.ToString("HH") + Constants.ANNOUNCE +
                DateTime.UtcNow.ToString("mm") + Constants.ANNOUNCE + Constants.WHITE_SPACE;
        }

        /// <summary>
        /// Area23DateTimeWithSeconds extension method for DateTime
        /// </summary>
        /// <param name="dateTime">d</param>
        /// <returns><see cref="string"/> formatted date time including seconds</returns>
        public static string Enabler4BizDateTimeWithSeconds(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd_HH:mm:ss");
        }

        public static string Enabler4BizDateTimeWithMillis(this DateTime dateTime)
        {
            string formatted = String.Format("{0:yyyyMMdd_HHmmss}_{1}", dateTime, dateTime.Millisecond);
            // return formatted;
            return dateTime.ToString("yyyyMMdd_HHmmss_") + dateTime.Millisecond;
        }

        #endregion DateTime extensions

        #region byte[] and stream extensions

        /// <summary>
        /// Extension method for <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream"><see cref="System.IO.Stream"/> which static methods are now extended</param>
        /// <returns>binary <see cref="byte[]">byte[] array</see></returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream is MemoryStream)
                return ((MemoryStream)stream).ToArray();
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }


        #endregion byte[] and stream extensions

        #region System.Exception extensions

        /// <summary>
        /// ToLogMsg - extension method to format an exception to a well formatted logging message
        /// </summary>
        /// <param name="exc">the <see cref="Exception">exception</see></param>
        /// <returns><see cref="string">logMsg</see></returns>
        public static string ToLogMsg(this Exception exc)
        {
            return string.Format("Exception {0} ⇒ {1}\t{2}\t{3}",
                    exc.GetType(),
                    exc.Message,
                    exc.ToString().Replace("\r", "").Replace("\n", " "),
                    exc.StackTrace.Replace("\r", "").Replace("\n", " "));
        }

        #endregion System.Exception extensions


    }
}
