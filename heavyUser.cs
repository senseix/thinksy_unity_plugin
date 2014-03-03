using System;
using System.Collections;
using System.Collections.Generic;

	public class heavyUser: user
	{
		public int id = -1;
		public string email = "null";
		public string name = "null";
		public int age = -1;
		public int coach_id = -1;
		public string created_at = "null";
		public string updated_at = "null";
		public int team_id = -1;
		public string deleted = "null";
		//FIXME: problem while input is string and output is integer and 
		public heavyUser()
		{
		
		}
		public heavyUser (string ida,string emaila,string namea,string agea,string coach_ida,string created_ata,string updated_ata,string team_ida,string deleteda)
		{
			if(ida.Equals("null"))
				id = -1;
			else
				id = Convert.ToInt32(ida);
			email = emaila;
			name=namea;
			if(agea.Equals("null"))
				age = -1;
			else
				age = Convert.ToInt32(agea);
			if(coach_ida.Equals("null"))
				coach_id = -1;
			else	
				coach_id = Convert.ToInt32(coach_ida);
			created_at=created_ata;
			updated_at=updated_ata;
			if(team_ida.Equals("null"))
				team_id = -1;
			else
				team_id = Convert.ToInt32(team_ida);
			deleted=deleteda;
		}
	}

