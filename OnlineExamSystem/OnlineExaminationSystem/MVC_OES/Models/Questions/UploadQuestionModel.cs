using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Questions
{
    public class UploadQuestionModel
    {
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}