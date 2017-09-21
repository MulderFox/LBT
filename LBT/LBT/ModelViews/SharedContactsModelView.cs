using System.ComponentModel.DataAnnotations;
using System.Linq;
using LBT.Models;
using LBT.Resources;

namespace LBT.ModelViews
{
    public class SharedContactsIndex : BaseModelView
    {
        public int SharedContactId { get; set; }

        public UserProfileDetails ToUser { get; set; }

        public SharedContactsIndex(SharedContact sharedContact)
        {
            SharedContactId = sharedContact.SharedContactId;
            ToUser = UserProfileDetails.GetModelView(sharedContact.ToUser);
        }

        public static SharedContactsIndex[] GetModelView(SharedContact[] sharedContacts)
        {
            if (sharedContacts == null)
                return null;

            var sharedContactsIndices = sharedContacts.Select(sc => new SharedContactsIndex(sc)).ToArray();
            return sharedContactsIndices;
        }
    }

    public class SharedContactsCreate : BaseModelView
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "SharedContact_ToUserId_Name", ResourceType = typeof(FieldResource))]
        public int ToUserId { get; set; }

        public SharedContactsCreate()
        {

        }

        public SharedContactsCreate(SharedContact sharedContact)
        {
            ToUserId = sharedContact.ToUserId;
        }

        public static SharedContactsCreate GetModelView(SharedContact sharedContact)
        {
            if (sharedContact == null)
                return null;

            var sharedContactsCreate = new SharedContactsCreate(sharedContact);
            return sharedContactsCreate;
        }

        public SharedContact GetModel(int userId)
        {
            var sharedContact = new SharedContact
                                    {
                                        FromUserId = userId,
                                        ToUserId = ToUserId
                                    };
            return sharedContact;
        }
    }

    public class SharedContactsDelete : BaseModelView
    {
        public UserProfileDelete ToUser { get; set; }

        public SharedContactsDelete(SharedContact sharedContact)
        {
            ToUser = UserProfileDelete.GetModelView(sharedContact.ToUser);
        }

        public static SharedContactsDelete GetModelView(SharedContact sharedContact)
        {
            if (sharedContact == null)
                return null;

            var sharedContactsDelete = new SharedContactsDelete(sharedContact);
            return sharedContactsDelete;
        }
    }
}