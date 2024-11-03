using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitCompiler.Models
{
    public class UserInputModel
    {
        public string Organization { get; set; }
        public string Project { get; set; }
        public string PersonalAccessToken { get; set; }
        public string SelectedRepository { get; set; }
        public string SelectedBranch { get; set; }
    }
}
