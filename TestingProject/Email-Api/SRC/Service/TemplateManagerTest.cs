using Email_Api.Model;
using Email_Api.Service;

namespace TestingProject.Email_Api.SRC.Service;
[TestFixture]
public class TemplateManagerTest
{
    private TemplateManager _templateManager;
    private string _testFolderPath;

    [SetUp]
    public void SetUp()
    {
        // Create a test folder
        _testFolderPath = "./TestTemplates";
        Directory.CreateDirectory(_testFolderPath);

        // Add sample test files
        File.WriteAllText(Path.Combine(_testFolderPath, "template1.html"), "Content for template1");
        File.WriteAllText(Path.Combine(_testFolderPath, "template2.html"), "Content for template2");
        File.WriteAllText(Path.Combine(_testFolderPath, "template3.html"), "Content for template3");

        // Initialize TemplateManager with a mock folder path
        _templateManager = new TemplateManager();
        typeof(TemplateManager)
            .GetField("_folderPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(_templateManager, _testFolderPath);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test folder
        Directory.Delete(_testFolderPath, true);
    }

    [Test]
    public void CountTxtFiles_ReturnsCorrectCount()
    {
        var count = _templateManager.CountTxtFiles();
        Assert.AreEqual(3, count);
    }

    [Test]
    public void ReadTxtFile_ReturnsFileContent()
    {
        var content = _templateManager.ReadTxtFile("template1.html");
        Assert.AreEqual("Content for template1", content);
    }

    [Test]
    public void ReadTxtFile_FileNotFound_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => _templateManager.ReadTxtFile("nonexistent.html"));
    }

    [Test]
    public void GetTemplate_ReturnsTemplate()
    {
        _templateManager.ReadAllTxtFiles();
        var template = _templateManager.GetTemplate("template1");
        Assert.AreEqual("Content for template1", template.HtmlFormat);
    }

    [Test]
    public void GetTemplate_TemplateNotFound_ThrowsKeyNotFoundException()
    {
        _templateManager.ReadAllTxtFiles();
        Assert.Throws<KeyNotFoundException>(() => _templateManager.GetTemplate("nonexistent"));
    }

    [Test]
    public void ReadAllTxtFiles_UpdatesFileContents()
    {
        _templateManager.ReadAllTxtFiles();
        var count = _templateManager.CountTxtFiles();
        Assert.That(_templateManager.GetType()
            .GetField("fileContents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_templateManager) is Dictionary<string, EmailTemplate> dictionary ? dictionary.Count : 0, Is.EqualTo(count));
    }

    [Test]
    public void GetTemplate_WhenNewFileAdded_UpdatesAutomatically()
    {
        _templateManager.ReadAllTxtFiles();

        // Add a new file
        File.WriteAllText(Path.Combine(_testFolderPath, "template4.html"), "Content for template4");

        // Ensure the new file is automatically loaded
        var template = _templateManager.GetTemplate("template4");
        Assert.AreEqual("Content for template4", template.HtmlFormat);
    }
}