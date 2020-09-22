using System;
using System.Collections.Generic;
using System.Linq;
using Headstone.Common;
using Headstone.Models;
using Headstone.Models.Events.Basket;
using Headstone.Models.Events.Campaign;
using Headstone.Models.Events.Comment;
using Headstone.Models.Events.Coupon;
using Headstone.Service;
using Headstone.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Headstone.Tests.Commands
{
    [TestClass]
    public class CommentCommands
    {
        private CommentService commentService = new CommentService();

        [TestMethod]
        public void CreateComment()
        {
            var commentCommand = new CommentCreated()
            {
                UserId = 1010,
                RelatedDataEntityType = "Test Product",
                RelatedDataEntityId = 1,
                Type = 0,
                Body = "TestBody"
            };

            var response = commentService.CreateComment(commentCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteComment()
        {
            var commentCommand = new CommentDeleted()
            {
                CommentId = 6
            };

            var response = commentService.DeleteComment(commentCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void UpdateComment()
        {
            var commentCommand = new CommentUpdated()
            {
                CommentId = 3,
                UserId = 1010,
                RelatedDataEntityId = 1,
                RelatedDataEntityType = "Updated Test Product",
                Rating = 3,
                Body = "Updated Test Body"
            };

            var response = commentService.UpdateComment(commentCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void UpdateCommentStatus()
        {
            var commentCommand = new CommentUpdated()
            {
                CommentId = 3,
                Status = Framework.Models.EntityStatus.Active
            };

            var response = commentService.UpdateCommentStatus(commentCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void CreateCommentProperty()
        {
            var commentCommand = new CommentPropertyCreated()
            {
                CommentId = 7,
                Environment = "test",
                ApplicationIP = "test",
                UserToken = "test",
                SessionId = "test"
            };

            var response = commentService.CreateCommentProperty(commentCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteCommentProperty()
        {
            var commentCommand = new CommentPropertyDeleted()
            {
                PropertyId = 4,
                CommentId = 7
            };

            var response = commentService.DeleteCommentProperty(commentCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void CreateCommentTag()
        {
            var commentCommand = new CommentTagCreated()
            {

                CommentId = 7,
                Environment = "test",
                ApplicationIP = "test",
                UserToken = "test",
                SessionId = "test"
            };

            var response = commentService.CreateCommentTag(commentCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteCommentTag()
        {
            var commentCommand = new CommentTagDeleted()
            {
                TagId = 4,
                CommentId = 7,
                Environment = "test",
                ApplicationIP = "test",
                UserToken = "test",
                SessionId = "test"
            };

            var response = commentService.DeleteCommentTag(commentCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

    }
}
