using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Comment;
using Headstone.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Interfaces
{
    public interface ICommentService
    {

        #region [ Queries ]

        CommentServiceResponse<Comment> GetComments(CommentRequest req, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ Comment ]

        CommentServiceResponse<Comment> CreateComment(CommentCreated ev, List<ServiceLogRecord> logRecords = null);
      
        CommentServiceResponse<Comment> UpdateComment(CommentUpdated ev, List<ServiceLogRecord> logRecords = null);

        CommentServiceResponse<Comment> UpdateCommentStatus(CommentUpdated ev, List<ServiceLogRecord> logRecords = null);
     
        CommentServiceResponse<Comment> DeleteComment(CommentDeleted ev, List<ServiceLogRecord> logRecords = null);

        #endregion
    }
}
