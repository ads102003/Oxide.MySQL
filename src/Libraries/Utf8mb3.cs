using System;
using System.Collections;
using MySql.Data.MySqlClient;

namespace CommandApp
{
    // ReSharper disable once InconsistentNaming
    /// Credit to Jeffraska 
    ///    https://github.com/jeffraska/Jf.MySql.Data.Collations
    ///    
    public static class Utf8mb3
	{
        // verified that starting version 6.10.0,  CharSetMap.mapping was changed to CharSetMap._mapping
        // code will still work if MySql.Data.dll is upgraded
        private static readonly Version NewFieldNamingVersion = new Version(6, 10, 0);
		
		public static void Enable()
		{
			// Add internal mapping of database utf8mb3 charset to .NET framework's UTF-8 encoding
			var assembly = System.Reflection.Assembly.GetAssembly(typeof(MySqlConnection));
			var connectorVersion = assembly.GetName().Version;
			
			var mappingFieldName = connectorVersion >= NewFieldNamingVersion ? "_mapping" : "mapping";
			
			var mappingField = assembly
				.GetType("MySql.Data.MySqlClient.CharSetMap").GetField(mappingFieldName,
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
					System.Reflection.BindingFlags.Static);

			if (mappingField != null)
			{
				var mappingDictionary = (IDictionary)mappingField.GetValue(null);
				var utf8Mapping = mappingDictionary["utf8"];

				if (utf8Mapping != null)
				{
					try
					{
						mappingDictionary.Add("utf8mb3", utf8Mapping);
                    }
					catch (ArgumentException)
					{
						// Item already exist
					}
				}
			}
		}
	}
}
