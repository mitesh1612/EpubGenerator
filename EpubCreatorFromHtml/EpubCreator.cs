using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;

namespace EpubCreatorFromHtml
{
    public class EpubCreator
    {
        private const string ContributorName = "Mitesh Shah's App"; // Change to app name

        public static void CreateEpub(string title, string creator)
        {
            // Fixes 'IBM437' is not a supported encoding name in the Zipping Methods
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Step 1 : Create the boiler plate files
            CreateEpubBoilerPlate(title, creator);
            Logger.LogToConsole("Boilerplate code for ePub created.");
            // Step 2 : Generate the Content HTML File 
            GenerateContentFile();
            // Step 2.5 : [Optional] Create a 'customizable' CSS File
            // Step 3 : Zip the file
            ZipUpTheEpub(title);
        }

        public static void CreateEpubBoilerPlate(string title, string creator)
        {
            // Step 1 : Create the MimeType File
            CreateMimeTypeFile();
            // Step 2 : Create the META-INF & Container File
            CreateMETAINFAndContents();
            // Step 3 : Create the OEBPS Folder
            CreateOEBPSFolder();
            // Step 4 : Create the Opf Metatadata file
            CreateMetadataOpfFile(title, creator);
            // Step 5 : Create the Toc Ncx File
            CreateTocNcxFile(title); // Should pass the same url as OPF File
        }

        private static void GenerateContentFile()
        {
            // TODO: Fix the dummy code
            // TODO: Should get it/fetch it. Currently using a static file
            var _destContentLocation = FileHelpers.GetFullFilePath(_contentFileName, _oebpsFolderName);
            var _destCoverLocation = FileHelpers.GetFullFilePath(_coverFileName, _oebpsFolderName);
            var _destSmallCoverLocation = FileHelpers.GetFullFilePath(_smallCoverFileName, _oebpsFolderName);
            FileHelpers.CopyFile(_contentFileName, _destContentLocation);
            FileHelpers.CopyFile(_coverFileName, _destCoverLocation);
            FileHelpers.CopyFile(_smallCoverFileName, _destSmallCoverLocation);
        }

        private static void ZipUpTheEpub(string title)
        {
            // TODO: Try simplifying the code using the second answer
            // Shamelessly ripped off from : https://stackoverflow.com/questions/5898787/creating-an-epub-file-with-a-zip-library
            string _outputFileName = $"{title}{_epubFileExtension}";
            using (FileStream fs = File.Open(_outputFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var output = new ZipOutputStream(fs))
                {
                    var e = output.PutNextEntry("mimetype");
                    e.CompressionLevel = CompressionLevel.None;
                    byte[] buffer = System.Text.Encoding.ASCII.GetBytes(_mimeTypeContent);
                    output.Write(buffer, 0, buffer.Length);
                    // Putting all the other files in zip
                    PutContentFilesInZipStream(output);
                }
            }
            Logger.LogToConsole("EPub Zipped Up & Saved Successfully.");
        }

        private static void PutContentFilesInZipStream(ZipOutputStream output)
        {
            var listOfFiles = new List<string>();
            listOfFiles.Add(FileHelpers.GetFullFilePath(_containerXmlFileName, _metaInfFolderName));
            listOfFiles.Add(FileHelpers.GetFullFilePath(_opfMetadataFileName, _oebpsFolderName));
            listOfFiles.Add(FileHelpers.GetFullFilePath(_tocFileName, _oebpsFolderName));
            listOfFiles.Add(FileHelpers.GetFullFilePath(_contentFileName, _oebpsFolderName));
            listOfFiles.Add(FileHelpers.GetFullFilePath(_coverFileName, _oebpsFolderName));
            listOfFiles.Add(FileHelpers.GetFullFilePath(_smallCoverFileName, _oebpsFolderName));
            foreach (var file in listOfFiles)
            {
                output.PutNextEntry(file);
                WriteExistingFile(output, file);
            }
        }

        private static void WriteExistingFile(Stream output, string fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                int n = -1;
                byte[] buffer = new byte[2048];
                while ((n = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, n);
                }
            }
        }

        private static void CreateTocNcxFile(string title, string url = "/")
        {
            // TODO: Try using XmlWriter class instead of StringBuilder
            var tocFileBuilder = new StringBuilder();
            tocFileBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>
            <ncx xmlns=""http://www.daisy.org/z3986/2005/ncx/"" version=""2005-1"">
            <head>");
            tocFileBuilder.AppendLine($@"<meta name=""{StringHelpers.ReplaceSpacesWithUnderscores(title)}"" content=""{title}"" scheme=""any"" />
            <meta name=""dtb:uid"" content=""{url}"" />");
            tocFileBuilder.AppendLine($@"</head>
              <docTitle>
                <text>{title}</text>
              </docTitle>");
            tocFileBuilder.AppendLine(GenerateNavMap(title));
            tocFileBuilder.AppendLine(@"</ncx>");
            var tocFileContent = tocFileBuilder.ToString();
            FileHelpers.CreateFileAndAddContent(tocFileContent, _tocFileName, _oebpsFolderName);
        }

        private static string GenerateNavMap(string title)
        {
            // Currently not reading the HTML to create chapters.
            // So getting only title and content here.
            // TODO: Take the content and generate a nice ToC (Future Enhancement)
            int playOrderValue = 1;
            var navMapBuilder = new StringBuilder();
            navMapBuilder.AppendLine(@$"<navMap>
            <navPoint id=""navpoint-{playOrderValue}"" playOrder=""{playOrderValue}"">
              <navLabel>
                <text>{title}</text>
              </navLabel>");
            playOrderValue++;
            navMapBuilder.AppendLine(@$"<content src=""{_contentFileName}""/>");
            navMapBuilder.AppendLine(@$"</navPoint>
            <navPoint id=""navpoint-{playOrderValue}"" playOrder=""{playOrderValue}"">
              <navLabel>
                <text>Contents</text>
              </navLabel>
              <content src=""{_contentFileName}""/>
            </navPoint>
            </navMap>");
            return navMapBuilder.ToString();
        }

        private static void CreateMetadataOpfFile(string title, string creator, string url = "/")
        {
            // TODO: Try using XmlWriter class instead of StringBuilder (Might handle spacing)
            var fileContentBuilder = new StringBuilder();
            fileContentBuilder.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>
            <package xmlns=""http://www.idpf.org/2007/opf"" unique-identifier=""url"" version=""2.0"">
            <metadata xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:opf=""http://www.idpf.org/2007/opf"">");
            fileContentBuilder.AppendLine($@"<dc:title>{title}</dc:title>");
            fileContentBuilder.AppendLine(@"<dc:language>en</dc:language>");
            fileContentBuilder.AppendLine($@"<dc:creator>{creator}</dc:creator>");
            fileContentBuilder.AppendLine($@"<dc:contributor>{ContributorName}</dc:contributor>");
            fileContentBuilder.AppendLine($@"<meta xmlns="""" name=""cover"" content=""cover"" />");
            fileContentBuilder.AppendLine($@"<dc:identifier id=""url"">{url}</dc:identifier>");
            fileContentBuilder.AppendLine(@"</metadata>");
            fileContentBuilder.AppendLine($@"<manifest>
            <item id=""content"" href=""{_contentFileName}"" media-type=""application/xhtml+xml"" />
            <item id=""ncx"" href=""{_tocFileName}"" media-type=""application/x-dtbncx+xml"" />
            <item id=""logo"" href=""{_logoFileName}"" media-type=""image/png"" />
            <item id=""cover_small"" href=""{_smallCoverFileName}"" media-type=""image/jpeg"" />
            <item id=""cover"" href=""{_coverFileName}"" media-type=""image/jpeg"" />
            </manifest>
            <spine toc=""ncx"">
            <itemref idref=""content"" />
            </spine>
            </package>");
            var fileContent = fileContentBuilder.ToString();
            FileHelpers.CreateFileAndAddContent(fileContent, _opfMetadataFileName, _oebpsFolderName);
        }

        private static void CreateOEBPSFolder()
        {
            FileHelpers.CreateFolderGivenNamePath(_oebpsFolderName);
        }

        private static void CreateMimeTypeFile()
        {
            FileHelpers.CreateFileAndAddContent(_mimeTypeContent, _mimetypeFileName);
        }

        private static void CreateMETAINFAndContents()
        {
            // Create a directory called META-INF
            FileHelpers.CreateFolderGivenNamePath(_metaInfFolderName);
            // Create & store the container.xml file
            // TODO: Handle spacing in this file
            FileHelpers.CreateFileAndAddContent(_containerXmlContent, _containerXmlFileName, _metaInfFolderName);
        }

        #region FileNames
        private const string _containerXmlFileName = "container.xml";
        private const string _mimetypeFileName = "mimetype";
        private const string _opfMetadataFileName = "metadata.opf"; // If you change this name, change the content of container.xml
        private const string _coverFileName = "cover.jpg";
        private const string _contentFileName = "content.html";
        private const string _smallCoverFileName = "cover_small.jpg";
        private const string _logoFileName = "logo.png";
        private const string _tocFileName = "toc.ncx";
        #endregion

        #region FolderName
        private const string _metaInfFolderName = "META-INF";
        private const string _oebpsFolderName = "OEBPS"; // If you change this name, change content of container.xml
        #endregion

        #region FileContents
        private const string _containerXmlContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <container xmlns=""urn:oasis:names:tc:opendocument:xmlns:container"" version=""1.0"">
            <rootfiles>
                <rootfile full-path=""OEBPS/metadata.opf"" media-type=""application/oebps-package+xml"" />
            </rootfiles>
        </container>";
        private const string _mimeTypeContent = "application/epub+zip";
        #endregion

        #region OtherConstantString
        private const string _epubFileExtension = ".epub";
        #endregion
    }
}
