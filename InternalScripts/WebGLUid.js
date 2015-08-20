import System.IO;

//var filePath = ""
function SetUdidCallback (param : String) 
{
   WriteFile("external_udid", param);
}
 
function WriteFile(fileName : String, param: String)
{
   // Debug.Log("Writing " + param + " To " + fileName);
    if (File.Exists(fileName)) { 
    //   Debug.Log("File already exists, skipping write");
       ReadFile(fileName); 
    }
    else
    {
    //   Debug.Log("File did not exist, creating it");
       var sw : StreamWriter = new StreamWriter(fileName);
       sw.WriteLine(param);
       sw.Flush();
       sw.Close();
     //  Debug.Log("Done writing the file");
    }
    
}
 
function ReadFile(fileName : String) {

   if (File.Exists(fileName))
   {
    var sr = new File.OpenText(fileName);
    var input = "";
    while (true) {
        input = sr.ReadLine();
        if (input == null) { break; }
      //  Debug.Log("line="+input);
    }
    sr.Close();
    return input;
   } 
   else
   {
     throw("File does not currently exist!"); 
   }
}