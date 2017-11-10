using System;
using System.Collections.Generic;
using CoiniumServ.Utils.Extensions;
using Serilog;

namespace CoiniumServ.Logging
{
    public class Loggee<T>:ILoggee
    {
        protected static readonly ILogger _logger=Log.ForContext<T>().ForContext("Component",typeof(T).Name);


        protected void LogMeSafelyHexString(IEnumerable<byte> obj, string objName = "someObject")
		{
			try
			{
				_logger.Debug("{0}={1}", objName, obj.ToHexString());
			}
			catch (Exception e)
			{
				_logger.Error("Don't worry, it's ok! Just tried to log the {0} as ToHexString() but exception occured: {1}",
							  objName, e.Message);
			}
		}

        virtual protected void DescribeYourself(){
            _logger.Error("Method \"void DescribeYourself()\" is not implemented!!!");
        }


        public void DescribeYourselfSafely(){
            try{
                DescribeYourself();
			}
			catch (Exception e)
			{
				_logger.Error("Something has happened while logging: ", e);
			}
        }
    }
}
