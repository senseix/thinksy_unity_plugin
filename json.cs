using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


	public class json
	{
		public const int JSON_MAX_LENGTH = 512;
		public json ()
		{

		}
		public static string Encoder(Dictionary<string,object> data)
		{
			StringBuilder container = new StringBuilder(JSON_MAX_LENGTH);
			container.Append("{");

			IDictionaryEnumerator de = data.GetEnumerator ();
			bool first = true;
			while (de.MoveNext()) {
				string key = de.Key.ToString();
				object value = de.Value;

				if (!first) {
					container.Append(", ");
				}
				//FIXME: would there be any problem with appending here?
			
				container.Append(key);
				container.Append(":");
				//FIXME: may have problem if the type is not string in future
				container.Append (value.ToString());

				first = false;
			}

			container.Append("}");
			return container.ToString ();
		}
	}

