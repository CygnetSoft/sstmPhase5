﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace SSTM.sg.com.eversafe.li {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Data;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="MyWebSoap", Namespace="http://tempuri.org/")]
    public partial class MyWeb : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback HelloWorldOperationCompleted;
        
        private System.Threading.SendOrPostCallback ShowTodayStartCoursesOperationCompleted;
        
        private System.Threading.SendOrPostCallback ShowTodayExamCoursesOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetStudentdetailsTodayCourseandBatchidOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetTrainerLoginOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetTodayclassTrainerOperationCompleted;
        
        private System.Threading.SendOrPostCallback SendSMSOperationCompleted;
        
        private System.Threading.SendOrPostCallback SentmailtoCustomerOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public MyWeb() {
            this.Url = global::SSTM.Properties.Settings.Default.SSTM_sg_com_eversafe_li_MyWeb;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event HelloWorldCompletedEventHandler HelloWorldCompleted;
        
        /// <remarks/>
        public event ShowTodayStartCoursesCompletedEventHandler ShowTodayStartCoursesCompleted;
        
        /// <remarks/>
        public event ShowTodayExamCoursesCompletedEventHandler ShowTodayExamCoursesCompleted;
        
        /// <remarks/>
        public event GetStudentdetailsTodayCourseandBatchidCompletedEventHandler GetStudentdetailsTodayCourseandBatchidCompleted;
        
        /// <remarks/>
        public event GetTrainerLoginCompletedEventHandler GetTrainerLoginCompleted;
        
        /// <remarks/>
        public event GetTodayclassTrainerCompletedEventHandler GetTodayclassTrainerCompleted;
        
        /// <remarks/>
        public event SendSMSCompletedEventHandler SendSMSCompleted;
        
        /// <remarks/>
        public event SentmailtoCustomerCompletedEventHandler SentmailtoCustomerCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/HelloWorld", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string HelloWorld() {
            object[] results = this.Invoke("HelloWorld", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HelloWorldAsync() {
            this.HelloWorldAsync(null);
        }
        
        /// <remarks/>
        public void HelloWorldAsync(object userState) {
            if ((this.HelloWorldOperationCompleted == null)) {
                this.HelloWorldOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHelloWorldOperationCompleted);
            }
            this.InvokeAsync("HelloWorld", new object[0], this.HelloWorldOperationCompleted, userState);
        }
        
        private void OnHelloWorldOperationCompleted(object arg) {
            if ((this.HelloWorldCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HelloWorldCompleted(this, new HelloWorldCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ShowTodayStartCourses", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ShowTodayStartCourses() {
            object[] results = this.Invoke("ShowTodayStartCourses", new object[0]);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void ShowTodayStartCoursesAsync() {
            this.ShowTodayStartCoursesAsync(null);
        }
        
        /// <remarks/>
        public void ShowTodayStartCoursesAsync(object userState) {
            if ((this.ShowTodayStartCoursesOperationCompleted == null)) {
                this.ShowTodayStartCoursesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnShowTodayStartCoursesOperationCompleted);
            }
            this.InvokeAsync("ShowTodayStartCourses", new object[0], this.ShowTodayStartCoursesOperationCompleted, userState);
        }
        
        private void OnShowTodayStartCoursesOperationCompleted(object arg) {
            if ((this.ShowTodayStartCoursesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ShowTodayStartCoursesCompleted(this, new ShowTodayStartCoursesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ShowTodayExamCourses", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ShowTodayExamCourses() {
            object[] results = this.Invoke("ShowTodayExamCourses", new object[0]);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void ShowTodayExamCoursesAsync() {
            this.ShowTodayExamCoursesAsync(null);
        }
        
        /// <remarks/>
        public void ShowTodayExamCoursesAsync(object userState) {
            if ((this.ShowTodayExamCoursesOperationCompleted == null)) {
                this.ShowTodayExamCoursesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnShowTodayExamCoursesOperationCompleted);
            }
            this.InvokeAsync("ShowTodayExamCourses", new object[0], this.ShowTodayExamCoursesOperationCompleted, userState);
        }
        
        private void OnShowTodayExamCoursesOperationCompleted(object arg) {
            if ((this.ShowTodayExamCoursesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ShowTodayExamCoursesCompleted(this, new ShowTodayExamCoursesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetStudentdetailsTodayCourseandBatchid", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetStudentdetailsTodayCourseandBatchid(int courseid, float batchid) {
            object[] results = this.Invoke("GetStudentdetailsTodayCourseandBatchid", new object[] {
                        courseid,
                        batchid});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetStudentdetailsTodayCourseandBatchidAsync(int courseid, float batchid) {
            this.GetStudentdetailsTodayCourseandBatchidAsync(courseid, batchid, null);
        }
        
        /// <remarks/>
        public void GetStudentdetailsTodayCourseandBatchidAsync(int courseid, float batchid, object userState) {
            if ((this.GetStudentdetailsTodayCourseandBatchidOperationCompleted == null)) {
                this.GetStudentdetailsTodayCourseandBatchidOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetStudentdetailsTodayCourseandBatchidOperationCompleted);
            }
            this.InvokeAsync("GetStudentdetailsTodayCourseandBatchid", new object[] {
                        courseid,
                        batchid}, this.GetStudentdetailsTodayCourseandBatchidOperationCompleted, userState);
        }
        
        private void OnGetStudentdetailsTodayCourseandBatchidOperationCompleted(object arg) {
            if ((this.GetStudentdetailsTodayCourseandBatchidCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetStudentdetailsTodayCourseandBatchidCompleted(this, new GetStudentdetailsTodayCourseandBatchidCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetTrainerLogin", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetTrainerLogin(string UsernameFin, string Password) {
            object[] results = this.Invoke("GetTrainerLogin", new object[] {
                        UsernameFin,
                        Password});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetTrainerLoginAsync(string UsernameFin, string Password) {
            this.GetTrainerLoginAsync(UsernameFin, Password, null);
        }
        
        /// <remarks/>
        public void GetTrainerLoginAsync(string UsernameFin, string Password, object userState) {
            if ((this.GetTrainerLoginOperationCompleted == null)) {
                this.GetTrainerLoginOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetTrainerLoginOperationCompleted);
            }
            this.InvokeAsync("GetTrainerLogin", new object[] {
                        UsernameFin,
                        Password}, this.GetTrainerLoginOperationCompleted, userState);
        }
        
        private void OnGetTrainerLoginOperationCompleted(object arg) {
            if ((this.GetTrainerLoginCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetTrainerLoginCompleted(this, new GetTrainerLoginCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetTodayclassTrainer", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetTodayclassTrainer(int courseid, float batchid) {
            object[] results = this.Invoke("GetTodayclassTrainer", new object[] {
                        courseid,
                        batchid});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetTodayclassTrainerAsync(int courseid, float batchid) {
            this.GetTodayclassTrainerAsync(courseid, batchid, null);
        }
        
        /// <remarks/>
        public void GetTodayclassTrainerAsync(int courseid, float batchid, object userState) {
            if ((this.GetTodayclassTrainerOperationCompleted == null)) {
                this.GetTodayclassTrainerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetTodayclassTrainerOperationCompleted);
            }
            this.InvokeAsync("GetTodayclassTrainer", new object[] {
                        courseid,
                        batchid}, this.GetTodayclassTrainerOperationCompleted, userState);
        }
        
        private void OnGetTodayclassTrainerOperationCompleted(object arg) {
            if ((this.GetTodayclassTrainerCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetTodayclassTrainerCompleted(this, new GetTodayclassTrainerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendSMS", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int SendSMS(string Mobileno, string Message) {
            object[] results = this.Invoke("SendSMS", new object[] {
                        Mobileno,
                        Message});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void SendSMSAsync(string Mobileno, string Message) {
            this.SendSMSAsync(Mobileno, Message, null);
        }
        
        /// <remarks/>
        public void SendSMSAsync(string Mobileno, string Message, object userState) {
            if ((this.SendSMSOperationCompleted == null)) {
                this.SendSMSOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendSMSOperationCompleted);
            }
            this.InvokeAsync("SendSMS", new object[] {
                        Mobileno,
                        Message}, this.SendSMSOperationCompleted, userState);
        }
        
        private void OnSendSMSOperationCompleted(object arg) {
            if ((this.SendSMSCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendSMSCompleted(this, new SendSMSCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SentmailtoCustomer", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int SentmailtoCustomer(string Message, string Subject, string ToMyemailid, string Mailcc) {
            object[] results = this.Invoke("SentmailtoCustomer", new object[] {
                        Message,
                        Subject,
                        ToMyemailid,
                        Mailcc});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void SentmailtoCustomerAsync(string Message, string Subject, string ToMyemailid, string Mailcc) {
            this.SentmailtoCustomerAsync(Message, Subject, ToMyemailid, Mailcc, null);
        }
        
        /// <remarks/>
        public void SentmailtoCustomerAsync(string Message, string Subject, string ToMyemailid, string Mailcc, object userState) {
            if ((this.SentmailtoCustomerOperationCompleted == null)) {
                this.SentmailtoCustomerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSentmailtoCustomerOperationCompleted);
            }
            this.InvokeAsync("SentmailtoCustomer", new object[] {
                        Message,
                        Subject,
                        ToMyemailid,
                        Mailcc}, this.SentmailtoCustomerOperationCompleted, userState);
        }
        
        private void OnSentmailtoCustomerOperationCompleted(object arg) {
            if ((this.SentmailtoCustomerCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SentmailtoCustomerCompleted(this, new SentmailtoCustomerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void HelloWorldCompletedEventHandler(object sender, HelloWorldCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HelloWorldCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HelloWorldCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void ShowTodayStartCoursesCompletedEventHandler(object sender, ShowTodayStartCoursesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ShowTodayStartCoursesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ShowTodayStartCoursesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataSet Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void ShowTodayExamCoursesCompletedEventHandler(object sender, ShowTodayExamCoursesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ShowTodayExamCoursesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ShowTodayExamCoursesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataSet Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void GetStudentdetailsTodayCourseandBatchidCompletedEventHandler(object sender, GetStudentdetailsTodayCourseandBatchidCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetStudentdetailsTodayCourseandBatchidCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetStudentdetailsTodayCourseandBatchidCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataSet Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void GetTrainerLoginCompletedEventHandler(object sender, GetTrainerLoginCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetTrainerLoginCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetTrainerLoginCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataSet Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void GetTodayclassTrainerCompletedEventHandler(object sender, GetTodayclassTrainerCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetTodayclassTrainerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetTodayclassTrainerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataSet Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void SendSMSCompletedEventHandler(object sender, SendSMSCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SendSMSCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SendSMSCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void SentmailtoCustomerCompletedEventHandler(object sender, SentmailtoCustomerCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SentmailtoCustomerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SentmailtoCustomerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591