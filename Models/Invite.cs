﻿using System.ComponentModel;

namespace JATS.Models
{
    public class Invite
    {

        public int Id { get; set; }

        [DisplayName("Date Sent")]
        public DateTimeOffset InviteDate { get; set; }


        [DisplayName("Join Date")]
        public DateTimeOffset dateTimeOffset { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Invitor")]
        public string InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string InviteeId { get; set; }

        [DisplayName("Invitee Email")]
        public string InviteeEmail { get; set; }

        [DisplayName("Invitee First Name")]
        public string InviteeFirstName { get; set; }

        [DisplayName("Invitee Last Name")]
        public string InviteeLastName { get; set; }


        public bool IsValid { get; set; }

        public virtual Company Company { get; set; }

        public virtual JTUser Invitor { get; set; }

        public virtual JTUser Invitee { get; set; }

        public virtual Project Project { get; set; }


    }
}
