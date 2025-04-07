namespace LMCM_BE.Utilities
{
    public static class ExpectedHeaders
    {
        public static readonly Dictionary<string, List<(string Header, string Cell)>> CurriculumImportHeaders =
            new()
            {
                { "Curriculum Subject", new List<(string, string)>
                    {
                        ("SubjectCode", "A1"),
                        ("SubjectName", "B1"),
                        ("English SubjectName", "C1"),
                        ("TermNo", "D1"),
                        ("Credits", "E1"),
                        ("Options", "F1")
                    }
                },
                { "Curriculum", new List<(string, string)>
                    {
                        ("No", "A1"),
                        ("Title", "B1"),
                        ("Details", "C1"),
                        ("Curriculum Code", "B2"),
                        ("Curriculum Name", "B3"),
                        ("English Curriculum Name", "B4"),
                        ("Curriculum Description", "B5"),
                        ("Vocational Code", "B6"),
                        ("Vocational Name", "B7"),
                        ("English Vocational Name", "B8"),
                        ("Decision No.", "B9"),
                        ("Approved date", "B10")
                    }
                },
                { "PLO", new List<(string, string)>
                    {
                        ("No", "A1"),
                        ("PLO Name", "B1"),
                        ("PLO Description", "C1")
                    }
                },
                { "PLO Mappings", new List<(string, string)>
                    {
                        ("Subject Code", "A2")
                    }
                }
            };
    }
}
