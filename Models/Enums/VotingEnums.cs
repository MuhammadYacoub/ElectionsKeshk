using System.ComponentModel.DataAnnotations;

namespace AdvancedVotingSystem.Models.Enums
{
    public enum InvalidationType
    {
        [Display(Name = "غير مفعل")]
        Disabled,
        [Display(Name = "جزئي")]
        Partial,
        [Display(Name = "كلي")]
        Total
    }

    public enum EntryMethod
    {
        [Display(Name = "الرقم القومي")]
        NationalId,
        [Display(Name = "كود دخول مجهول")]
        UnknownCode,
        [Display(Name = "كود دخول معلوم")]
        KnownCode
    }

    public enum CategoryType
    {
        [Display(Name = "نظام المقاعد")]
        SeatBased,
        [Display(Name = "رأي فردي")]
        IndividualOpinion,
        [Display(Name = "لائحة")]
        Regulation
    }

    public enum ElectionStatus
    {
        [Display(Name = "نشط")]
        Active,
        [Display(Name = "موقوف")]
        Paused,
        [Display(Name = "مغلق")]
        Closed
    }

    public enum CommitteeStatus
    {
        [Display(Name = "مفتوح")]
        Open,
        [Display(Name = "مغلق")]
        Closed
    }

    public enum ElectionType
    {
        [Display(Name = "لائحة")]
        Bylaw,
        [Display(Name = "مقاعد")]
        Positions,
        [Display(Name = "جمعية عمومية")]
        GeneralAssembly
    }
}
