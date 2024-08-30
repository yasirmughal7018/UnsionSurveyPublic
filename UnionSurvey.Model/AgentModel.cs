using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class AgentModel
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string AgentName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = "";

        public bool AgentStatus { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = "";
    }

    public class ChatUserModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }

    }
}
