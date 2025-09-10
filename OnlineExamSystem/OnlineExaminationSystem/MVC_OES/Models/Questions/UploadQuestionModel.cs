using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Questions
{
    public class UploadQuestionModel
    {
        [Required]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}