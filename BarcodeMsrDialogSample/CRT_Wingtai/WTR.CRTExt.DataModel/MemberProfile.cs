
namespace WTR.CRT.DataModel
{
    using System.Collections.Generic;
    using System;

    /// <summary>
    /// Defines a simple class that holds information about opening and closing times for a particular day.
    /// </summary>
    /// 

    public class MemberProfile
    {
        public string MemberAutoID { get; set; }
        public string MemberID { get; set; }
        public string Salutation { get; set; }
        public string Name { get; set; }
        public string NRIC { get; set; }
        public string Passport { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string Nationality { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Unit { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string ContactNo { get; set; }
        public string MobileNo { get; set; }
        public string FaxNo { get; set; }
        public string ReferrerCode { get; set; }
        public string FacebookID { get; set; }
        public string FacebookName { get; set; }
        public string FacebookPhotoLink { get; set; }
        public string FacebookToken { get; set; }
        public string FacebookTokenExpiry { get; set; }
        public string FullPhotoName { get; set; }
        public string Base64PhotoString { get; set; }
        public string PhotoLink { get; set; }
        public object InterestGroupLists { get; set; }
        public List<MailingList> MailingLists { get; set; }
        public List<DynamicColumnList> DynamicColumnLists { get; set; }
        public List<DynamicFieldList> DynamicFieldLists { get; set; }
        public DateTime JoinDate { get; set; }
        public object LastLoginDateTime { get; set; }
        public List<ActiveTagList> ActiveTagList { get; set; }
        public WTR_MemberCardTemplate WTR_MemberCardTemplate { get; set; }
        public string MembershipStatus { get; set; }
        public string MembershipType { get; set; }
        public string MembershipTier { get; set; }
        public string CardNo { get; set; }
        public string DollarToPointsRatio { get; set; }

    }

    public class DynamicColumnList
    {
        public string Name { get; set; }
        public string ColValue { get; set; }
        public string Type { get; set; }
    }

    public class RequestDynamicColumnList
    {
        public string Name { get; set; }
    }

    public class DynamicFieldList
    {
        public string Name { get; set; }
        public string ColValue { get; set; }
        public string Type { get; set; }
    }
    public class RequestDynamicFieldList
    {
        public string Name { get; set; }
    }
    public class MailingList
    {
        public string Name { get; set; }
        public MailingList(string _name)
        {
            this.Name = _name;
        }
    }

    public class ActiveTagList
    {
        public int TagAutoId { get; set; }
        public string TagName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}