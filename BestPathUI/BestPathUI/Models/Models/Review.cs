using Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
    public class Review : BaseModel
    {
        public string ReviewComment { get; set; }
        public int Stars { get; set; }
        public new ReviewDTO GetDTO()
        {
            return new ReviewDTO
            {
                Id = this.Id,
                Stars = this.Stars,
                ReviewComment = this.ReviewComment
            };
        }
    }
}
