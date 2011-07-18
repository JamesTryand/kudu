            Assert.Throws<InvalidOperationException>(() => GitExeRepository.ConvertStatus("AG"));
            ChangeType addDeleted = GitExeRepository.ConvertStatus("AD");
            Assert.Equal(ChangeType.Deleted, addDeleted);
        [Fact]
        public void ParseDiffFileName() {
            string singleCharFileName = GitExeRepository.ParseFileName("git --diff a/a b/a");
            string evenNumberFileName = GitExeRepository.ParseFileName("git --diff a/aa b/aa");
            string moreAmbiguous = GitExeRepository.ParseFileName("git --diff a/ b  b/ b ");
            string fileNameWithSpaces = GitExeRepository.ParseFileName("git --diff a/New File b/New File");
            string fileNameWithSlashB = GitExeRepository.ParseFileName("git --diff a/foo/bar/lib/a.dll b/foo/bar/lib/a.dll");
            string ambiguous = GitExeRepository.ParseFileName("diff --git a/Folder b/blah.txt b/Folder b/blah.txt");


            Assert.Equal("a", singleCharFileName);
            Assert.Equal("aa", evenNumberFileName);
            Assert.Equal("New File", fileNameWithSpaces);
            Assert.Equal("foo/bar/lib/a.dll", fileNameWithSlashB);
            Assert.Equal("Folder b/blah.txt", ambiguous);
            Assert.Equal(" b ", moreAmbiguous);
        }
