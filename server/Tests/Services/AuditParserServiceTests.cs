using Microsoft.VisualStudio.TestTools.UnitTesting;
using server.Models;
using server.Services;
using server.Tests.TestHelpers;

namespace server.Tests.Services;

[TestClass]
public class AuditParserServiceTests
{
    [TestMethod]
    public void Parse_ReturnsSectionsRequirementsAndCourses_FromPdfText()
    {
        AuditParserService service = new AuditParserService();

        using MemoryStream pdfStream = PdfTestHelper.CreateSinglePagePdf(
            "FIU UNIVERSITY CORE CURRICULUM",
            "Status: Satisfied",
            "COMMUNICATION",
            "Status: Satisfied",
            "The following courses were used to satisfy this requirement:",
            "Course Description Units When Grade Type Repeat",
            "ENC1101 Writing and Rhetoric I 3.00 FALL 2021 A TR");

        Audit audit = service.Parse(pdfStream);

        Assert.AreEqual(1, audit.Sections.Count);
        Assert.AreEqual("FIU UNIVERSITY CORE CURRICULUM", audit.Sections[0].Title);
        Assert.IsTrue(audit.Sections[0].IsSatisfied);

        Assert.AreEqual(1, audit.Sections[0].Requirements.Count);
        AuditRequirement requirement = audit.Sections[0].Requirements[0];
        Assert.AreEqual("COMMUNICATION", requirement.Title);
        Assert.IsTrue(requirement.IsSatisfied);
        Assert.AreEqual(1, requirement.CoursesTaken.Count);
        Assert.AreEqual("ENC1101", requirement.CoursesTaken[0].Code);
        Assert.AreEqual("Writing and Rhetoric I", requirement.CoursesTaken[0].Title);
    }
}