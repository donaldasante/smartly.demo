/* Global variables */
import { install } from 'riot';

export var posts = []

export var storage = {
    init: function() {
        if (localStorage.getItem('posts') === 'undefined' || localStorage.getItem('posts') == null) {
            localStorage.setItem('posts', '[]');
        }

        posts = JSON.parse(localStorage.getItem('posts'));

    },

    getPost: function(Component) {        
        if (Component.props && Component.props.index) {
            for (var i in posts) {
                if (Number(posts[i].index) === Number(Component?.props?.index)) {
                    Component.post = posts[i];
                    Component.post.index = i;
                }
            }
        }
    },

    delete: function(postParam) {
        posts.splice(postParam.index,1);
        for (let i =0; i < posts.length;i++)
        {
            posts[i].index  = i;
        }
        let posts_str = JSON.stringify(posts);
        localStorage.setItem('posts', posts_str);
    },

    insert: function(postParam)
    {
        let lastIndex = posts && posts?.length > 0?posts.length:0;
        postParam.index = Number(lastIndex);
        posts.push(postParam);
        let posts_str = JSON.stringify(posts);
        localStorage.setItem('posts', posts_str);
    },

    update: function(postParam)
    {
        posts[Number(postParam.index)] = postParam;
        let posts_str = JSON.stringify(posts);
        localStorage.setItem('posts', posts_str);
    }
}


install(function(component) {
    component.posts = posts;
    component.storage = storage;
    return component;
})