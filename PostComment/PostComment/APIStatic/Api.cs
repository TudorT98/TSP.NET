using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace PostComment.APIStatic
{
    public static class Api
    {
        public static bool AddPost(Post post)
        {
            using (PostCommentContainer ctx = new PostCommentContainer())
            {
                bool bResult = false;
                if (post.Id == 0)
                {
                    var it = ctx.Entry<Post>(post).State = EntityState.Added;
                    ctx.SaveChanges();
                    bResult = true;
                }
                return bResult;
            }
        }
        public static Post UpdatePost(Post newPost)
        {
            using (PostCommentContainer ctx = new PostCommentContainer())
            {
                // Ce e in bd. PK nu poate fi modificata
                Post oldPost = ctx.PostSet.Find(newPost.Id);
                if (oldPost == null) // nu exista in bd
                {
                    return null;
                }
                oldPost.Description = newPost.Description;
                oldPost.Domain = newPost.Domain;
                oldPost.Date = newPost.Date;
                ctx.SaveChanges();
                return oldPost;
            }
        }
        public static int DeletePost(int id)
        {
            using (PostCommentContainer ctx = new PostCommentContainer())
            {
                return ctx.Database.ExecuteSqlCommand("Delete From PostSet where Id = @p0", id);
            }
        }
        public static Post GetPostById(int id)
        {
            using (PostCommentContainer ctx = new PostCommentContainer())
            {
                var items = from p in ctx.PostSet where (p.Id == id) select p;
                if (items != null)
                    return items.Include(c => c.Comment).SingleOrDefault();
                return null;
            }
        }
    
        public static List<Post> GetAllPosts()
        {
            using (PostCommentContainer ctx = new PostCommentContainer())
            {
                return ctx.PostSet.Include("Comment").ToList<Post>();
            }
        }
        // Comment table
        public static bool AddComment(Comment comment)
        {
            using (PostCommentContainer ctx = new PostCommentContainer())
            {
                bool bResult = false;
                if (comment == null || comment.PostId == 0)
                    return bResult;
                if (comment.Id == 0)
                {
                    ctx.Entry<Comment>(comment).State = EntityState.Added;
                    Post p = ctx.PostSet.Find(comment.PostId);
                    ctx.Entry<Post>(p).State = EntityState.Unchanged;
                    ctx.SaveChanges();
                    bResult = true;
                }
                return bResult;
            }
        }
        public static Comment UpdateComment(Comment newComment)
        {
            using (PostCommentContainer ctx = new PostCommentContainer())
            {
                Comment oldComment = ctx.CommentSet.Find(newComment.Id);
                if (newComment.Text != null)
                    oldComment.Text = newComment.Text;
                if ((oldComment.PostId != newComment.PostId)
               && (newComment.PostId != 0))
                {
                    oldComment.PostId = newComment.PostId;
                }
                ctx.SaveChanges();
                return oldComment;
            }
        }
        public static Comment GetCommentById(int id)
        {
            using (PostCommentContainer ctx = new PostCommentContainer())
            {
                var items = from c in ctx.CommentSet where (c.Id == id) select c;
                return items.Include(p => p.Post).SingleOrDefault();
            }
        }
    }
}