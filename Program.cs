using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;




namespace eBookFix
{

class Program
{

string opfFile = "";
string epubFile = "";
string replaceLine = "<meta property=\"rendition:spread\">both</meta>";
string replaceWord = "both";
string correctWord = "auto";
int epubsFound = 0;
int epubsEdited = 0;
String[] rows;




public static void Main(string[] args)
{
	var programInstance = new Program();
	programInstance.GetEpubFiles();
	Console.ReadLine();
}


public void GetEpubFiles()
{
	string[] zipFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.epub");
	foreach (string epubPath in zipFilePaths)
	{
		epubFile = Path.GetFileName(epubPath);
		GetOpfFile();
		EditOpfFile();
		epubsFound++;
	}
	Console.WriteLine(epubsFound + " ePub Dateien gefunden");
	Console.WriteLine(epubsEdited + " ePub Dateien geändert");
}

public void GetOpfFile()
{
	using (ZipArchive archive = ZipFile.OpenRead(epubFile))
	{
		foreach (ZipArchiveEntry entry in archive.Entries)
		{
			if (entry.FullName.EndsWith(".opf"))
			{
				opfFile = entry.FullName;
			}
		}
	}
}



public void EditOpfFile()
{
	using (ZipArchive archive = ZipFile.Open(epubFile, ZipArchiveMode.Update))
	{
		ZipArchiveEntry entry = archive.GetEntry(opfFile);
		using (StreamReader reader = new StreamReader(entry.Open()))
		{
			rows = Regex.Split(reader.ReadToEnd(), "\r\n");
			for (int i = 0; i < rows.Length; i++)
			{
				if (rows[i].Contains(replaceLine))
				{
					reader.Close();
					using (StreamWriter writer = new StreamWriter(entry.Open()))
					{
						for (int x = 0; x < rows.Length; x++)
						{
							if (x == i)
							{
								rows[x] = rows[x].Replace(replaceWord, correctWord);
								epubsEdited++;
							}
							writer.WriteLine(rows[x]);
						}
						writer.Close();
					}
					//entry.LastWriteTime = DateTimeOffset.UtcNow.LocalDateTime;
					return;
				}
			}
			reader.Close();
		}


	}
}


}

}