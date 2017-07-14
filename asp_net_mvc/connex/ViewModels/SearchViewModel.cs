using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class SearchViewModel
    {
        public string id { get; set; }
        public List<Event> events;
        public List<Post> posts;
        public List<Gallery> galleries;
        public List<Document> documents;
        public List<Video> videos;
        
        public bool Inc_Events;
        public bool Inc_Posts;
        public bool Inc_Galleries;
        public bool Inc_Documents;
        public bool Inc_Videos;

        public SearchViewModel()
        {
            this.id = "";
            this.events = new List<Event>();
            this.posts = new List<Post>();
            this.galleries = new List<Gallery>();
            this.documents = new List<Document>();
            this.videos = new List<Video>();

            this.Inc_Posts = true;
            this.Inc_Galleries = true;
            this.Inc_Events = true;
            this.Inc_Documents = true;
            this.Inc_Videos = true;
        }
        public SearchViewModel(string param, bool Inc_Posts, bool Inc_Galleries, bool Inc_Events, bool Inc_Documents, bool Inc_Videos)
        {
            CONNEXDBEntities db = new CONNEXDBEntities();
            this.id = param;
            if (param != "")
            {
                this.Inc_Posts = Inc_Posts;
                this.Inc_Galleries = Inc_Galleries;
                this.Inc_Events = Inc_Events;
                this.Inc_Documents = Inc_Documents;
                this.Inc_Videos = Inc_Videos;

                string[] paramParts = param.Split(' ');

                if (Inc_Events)
                {
                    this.events = (List<Event>)db.Events.Where(x => x.Archived == false).ToList();
                }
                else
                {
                    this.events = new List<Event>();
                }
                if (Inc_Posts)
                {
                    this.posts = (List<Post>)db.Posts.Where(x => x.Archived == false).ToList();
                }
                else
                {
                    this.posts = new List<Post>();
                }
                if (Inc_Galleries)
                {
                    this.galleries = (List<Gallery>)db.Galleries.Where(x => x.Archived == false).ToList();
                }
                else
                {
                    this.galleries = new List<Gallery>();

                }
                if (Inc_Documents)
                {
                    this.documents = (List<Document>)db.Documents.Where(x => x.Archived == false).ToList();
                }
                else
                {
                    this.documents = new List<Document>();
                }
                if (Inc_Videos)
                {
                    this.videos = (List<Video>)db.Videos.Where(x => x.Archived == false).ToList();
                }
                else
                {
                    this.videos = new List<Video>();
                }

                foreach (string part in paramParts)
                {
                    this.events = this.events.Where(x => x.Event_Name.ToLower().Contains(part.ToLower()) || x.Description.ToLower().Contains(part.ToLower()) || x.Location.ToLower().Contains(part.ToLower())).ToList();
                    this.posts = this.posts.Where(x => x.Header.ToLower().Contains(part.ToLower()) || x.Summary.ToLower().Contains(part.ToLower())).ToList();
                    ///TODO:allow searching within post text. need to handle null values
                    this.galleries = this.galleries.Where(x => x.Gallery_Name.ToLower().Contains(part.ToLower())).ToList();
                    this.documents = this.documents.Where(x => x.Name.ToLower().Contains(part.ToLower()) || x.Path.ToLower().Contains(part.ToLower()) || x.Library.Title_Text.ToLower().Contains(part.ToLower())).ToList();
                    this.videos = this.videos.Where(x => x.Caption.ToLower().Contains(part.ToLower()) || x.File_Path.ToLower().Contains(part.ToLower()) || x.Thumb_Path.ToLower().Contains(part.ToLower()) || x.Title_Text.ToLower().Contains(part.ToLower()) || x.VideoGallery.Gallery_Name.ToLower().Contains(part.ToLower())).ToList();
                }
            }
            else
            {
                this.events = new List<Event>();
                this.posts = new List<Post>();
                this.galleries = new List<Gallery>();
                this.documents = new List<Document>();
                this.videos = new List<Video>();
            }


        }
    }
}