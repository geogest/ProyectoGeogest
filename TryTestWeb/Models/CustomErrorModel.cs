using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CustomErrorModel
{
    public string ErrorMessage { get; set; }

    public CustomErrorModel()
    {

    }

    public CustomErrorModel(string _errorMessage)
    {
        this.ErrorMessage = _errorMessage;
    }
}
