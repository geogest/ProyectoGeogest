﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=4.0.30319.17929.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name = "GetTokenFromSeedSoapBinding", Namespace = "http://DefaultNamespace")]
public partial class GetTokenFromSeedProdService : System.Web.Services.Protocols.SoapHttpClientProtocol
{

    private System.Threading.SendOrPostCallback getStateOperationCompleted;

    private System.Threading.SendOrPostCallback getVersionMayorOperationCompleted;

    private System.Threading.SendOrPostCallback getVersionMenorOperationCompleted;

    private System.Threading.SendOrPostCallback getVersionPatchOperationCompleted;

    private System.Threading.SendOrPostCallback getTokenOperationCompleted;

    /// <remarks/>
    public GetTokenFromSeedProdService(bool useProd)
    {
        if (useProd)
        {
            this.Url = "https://palena.sii.cl/DTEWS/GetTokenFromSeed.jws";
        }
        else
        {
            this.Url = "https://maullin.sii.cl/DTEWS/GetTokenFromSeed.jws";
        }
    }

    /// <remarks/>
    public event getStateCompletedEventHandler getStateCompleted;

    /// <remarks/>
    public event getVersionMayorCompletedEventHandler getVersionMayorCompleted;

    /// <remarks/>
    public event getVersionMenorCompletedEventHandler getVersionMenorCompleted;

    /// <remarks/>
    public event getVersionPatchCompletedEventHandler getVersionPatchCompleted;

    /// <remarks/>
    public event getTokenCompletedEventHandler getTokenCompleted;

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://DefaultNamespace", ResponseNamespace = "http://DefaultNamespace")]
    [return: System.Xml.Serialization.SoapElementAttribute("getStateReturn")]
    public string getState()
    {
        object[] results = this.Invoke("getState", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetState(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getState", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetState(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getStateAsync()
    {
        this.getStateAsync(null);
    }

    /// <remarks/>
    public void getStateAsync(object userState)
    {
        if ((this.getStateOperationCompleted == null))
        {
            this.getStateOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetStateOperationCompleted);
        }
        this.InvokeAsync("getState", new object[0], this.getStateOperationCompleted, userState);
    }

    private void OngetStateOperationCompleted(object arg)
    {
        if ((this.getStateCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getStateCompleted(this, new getStateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://DefaultNamespace", ResponseNamespace = "http://DefaultNamespace")]
    [return: System.Xml.Serialization.SoapElementAttribute("getVersionMayorReturn")]
    public string getVersionMayor()
    {
        object[] results = this.Invoke("getVersionMayor", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetVersionMayor(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getVersionMayor", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetVersionMayor(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getVersionMayorAsync()
    {
        this.getVersionMayorAsync(null);
    }

    /// <remarks/>
    public void getVersionMayorAsync(object userState)
    {
        if ((this.getVersionMayorOperationCompleted == null))
        {
            this.getVersionMayorOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetVersionMayorOperationCompleted);
        }
        this.InvokeAsync("getVersionMayor", new object[0], this.getVersionMayorOperationCompleted, userState);
    }

    private void OngetVersionMayorOperationCompleted(object arg)
    {
        if ((this.getVersionMayorCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getVersionMayorCompleted(this, new getVersionMayorCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://DefaultNamespace", ResponseNamespace = "http://DefaultNamespace")]
    [return: System.Xml.Serialization.SoapElementAttribute("getVersionMenorReturn")]
    public string getVersionMenor()
    {
        object[] results = this.Invoke("getVersionMenor", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetVersionMenor(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getVersionMenor", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetVersionMenor(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getVersionMenorAsync()
    {
        this.getVersionMenorAsync(null);
    }

    /// <remarks/>
    public void getVersionMenorAsync(object userState)
    {
        if ((this.getVersionMenorOperationCompleted == null))
        {
            this.getVersionMenorOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetVersionMenorOperationCompleted);
        }
        this.InvokeAsync("getVersionMenor", new object[0], this.getVersionMenorOperationCompleted, userState);
    }

    private void OngetVersionMenorOperationCompleted(object arg)
    {
        if ((this.getVersionMenorCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getVersionMenorCompleted(this, new getVersionMenorCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://DefaultNamespace", ResponseNamespace = "http://DefaultNamespace")]
    [return: System.Xml.Serialization.SoapElementAttribute("getVersionPatchReturn")]
    public string getVersionPatch()
    {
        object[] results = this.Invoke("getVersionPatch", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetVersionPatch(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getVersionPatch", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetVersionPatch(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getVersionPatchAsync()
    {
        this.getVersionPatchAsync(null);
    }

    /// <remarks/>
    public void getVersionPatchAsync(object userState)
    {
        if ((this.getVersionPatchOperationCompleted == null))
        {
            this.getVersionPatchOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetVersionPatchOperationCompleted);
        }
        this.InvokeAsync("getVersionPatch", new object[0], this.getVersionPatchOperationCompleted, userState);
    }

    private void OngetVersionPatchOperationCompleted(object arg)
    {
        if ((this.getVersionPatchCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getVersionPatchCompleted(this, new getVersionPatchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://DefaultNamespace", ResponseNamespace = "http://DefaultNamespace")]
    [return: System.Xml.Serialization.SoapElementAttribute("getTokenReturn")]
    public string getToken(string pszXml)
    {
        object[] results = this.Invoke("getToken", new object[] {
                    pszXml});
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetToken(string pszXml, System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getToken", new object[] {
                    pszXml}, callback, asyncState);
    }

    /// <remarks/>
    public string EndgetToken(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getTokenAsync(string pszXml)
    {
        this.getTokenAsync(pszXml, null);
    }

    /// <remarks/>
    public void getTokenAsync(string pszXml, object userState)
    {
        if ((this.getTokenOperationCompleted == null))
        {
            this.getTokenOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetTokenOperationCompleted);
        }
        this.InvokeAsync("getToken", new object[] {
                    pszXml}, this.getTokenOperationCompleted, userState);
    }

    private void OngetTokenOperationCompleted(object arg)
    {
        if ((this.getTokenCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getTokenCompleted(this, new getTokenCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
public delegate void getTokenCompletedEventHandler(object sender, getTokenCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class getTokenCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal getTokenCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
        base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public string Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}

    /// <remarks/>
    public new void CancelAsync(object userState)
    {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
public delegate void getVersionCompletedEventHandler(object sender, getVersionCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class getVersionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal getVersionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
        base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public string Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}

