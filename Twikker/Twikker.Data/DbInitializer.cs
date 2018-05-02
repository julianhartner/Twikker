using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Twikker.Data.Models;

namespace Twikker.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TwikkerDataContext context)
        {
            context.Database.EnsureCreated();

            if (context.Posts.Any())
                return;

            var users = new[]
            {
                new ApplicationUser
                {
                    UserName = "MarkusDichter",
                    NormalizedUserName = "MARKUSDICHTER",
                    Email = "markus.dichter@email.com",
                    NormalizedEmail = "MARKUS.DICHTER@EMAIL.COM",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                },
                new ApplicationUser
                {
                    UserName = "FranzHuber",
                    NormalizedUserName = "FRANZHUBER",
                    Email = "franz.huber@email.com",
                    NormalizedEmail = "FRANZ.HUBER@EMAIL.COM",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                },
                new ApplicationUser
                {
                    UserName = "HansSchmied",
                    NormalizedUserName = "HANSSCHMIED",
                    Email = "hans.schmied@email.com",
                    NormalizedEmail = "HANS.SCHMIED@EMAIL.COM",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                },
                new ApplicationUser
                {
                    UserName = "BernhardBauer",
                    NormalizedUserName = "BERNHARDBAUER",
                    Email = "bernhard.bauer@email.com",
                    NormalizedEmail = "BERNHARD.BAUER@EMAIL.COM",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                },
                new ApplicationUser
                {
                    UserName = "HubertSchneider",
                    NormalizedUserName = "HUBERTSCHNEIDER",
                    Email = "hubert.schneider@email.com",
                    NormalizedEmail = "HUBERT.SCHNEIDER@EMAIL.COM",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                }
            };

            foreach (var user in users)
                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(user, "Password123!");
                    user.PasswordHash = hashed;

                    var userStore = new UserStore<ApplicationUser>(context);
                    var result = userStore.CreateAsync(user);
                }

            InitializePosts(context);
        }

        private static async void InitializePosts(TwikkerDataContext context)
        {
            var applicationUsers = await context.Users.ToListAsync();
            var dichter = applicationUsers[0];
            var huber = applicationUsers[1];
            var schmied = applicationUsers[2];
            var bauer = applicationUsers[3];
            var schneider = applicationUsers[4];

            var comment1 = new TwikkerComment
            {
                Content =
                    "People have got the impression that the merino is a gentle, bleating animal that gets its living without trouble to anybody, and comes up every year to be shorn with a pleased smile upon its amiable face. It is my purpose here to exhibit the merino sheep in its true light.",
                Owner = dichter,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment2 = new TwikkerComment
            {
                Content =
                    "First let us give him his due. No one can accuse him of being a ferocious animal. No one could ever say that a sheep attacked him without provocation; although there is an old bush story of a man who was discovered in the act of killing a neighbour's wether.",
                Owner = huber,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment3 = new TwikkerComment
            {
                Content =
                    "The truth is that he is a dangerous monomaniac, and his one idea is to ruin the man who owns him. With this object in view he will display a talent for getting into trouble and a genius for dying that are almost incredible.",
                Owner = schmied,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment4 = new TwikkerComment
            {
                Content = "That, sir, replied the astounded functionary -- that IS Sir Oliver, sir",
                Owner = bauer,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment5 = new TwikkerComment
            {
                Content =
                    "A mob will crawl along a road slowly enough to exasperate a snail, but let a lamb get away in a bit of rough country, and a racehorse can't head him back again.",
                Owner = schneider,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment6 = new TwikkerComment
            {
                Content =
                    "When being counted out at a gate, if a scrap of bark be left on the ground in the gateway, they will refuse to step over it until dogs and men have sweated and toiled and sworn and heeled \'em up, and spoke to \'em, and fairly jammed them at it.",
                Owner = dichter,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment7 = new TwikkerComment
            {
                Content =
                    "At last one will gather courage, rush at the fancied obstacle, spring over it about six feet in the air, and dart away.",
                Owner = huber,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment8 = new TwikkerComment
            {
                Content = "Then the dogging and shrieking and hustling and tearing have to be gone through all over again.",
                Owner = schmied,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment9 = new TwikkerComment
            {
                Content =
                    "A sheep won't go through an open gate on his own responsibility, but he would gladly and proudly follow the leader through the red-hot portals of Hades.",
                Owner = bauer,
                PostDate = DateTime.Now.AddHours(5)
            };
            var comment10 = new TwikkerComment
            {
                Content =
                    "The fiendish resemblance which one sheep bears to another is a great advantage to them in their struggles with their owners.",
                Owner = schneider,
                PostDate = DateTime.Now.AddHours(5)
            };


            var post1 = new TwikkerPost
            {
                Content =
                    "For several days in succession fragments of a defeated army had passed through the town. They were mere disorganized bands, not disciplined forces.",
                Comments = new List<TwikkerComment> {comment1, comment2, comment3},
                PostDate = DateTime.Now,
                Owner = dichter
            };

            var post2 = new TwikkerPost
            {
                Content = "The men wore long, dirty beards and tattered uniforms;",
                PostDate = DateTime.Now,
                Owner = schmied
            };

            var post3 = new TwikkerPost
            {
                Content =
                    "All seemed exhausted, worn out, incapable of thought or resolve, marching onward merely by force of habit, and dropping to the ground with fatigue the moment they halted.",
                PostDate = DateTime.Now,
                Owner = bauer
            };

            var post4 = new TwikkerPost
            {
                Content = "Then a profound calm, a shuddering, silent dread, settled on the city.",
                Comments = new List<TwikkerComment> {comment4, comment5},
                PostDate = DateTime.Now,
                Owner = schneider
            };

            var post5 = new TwikkerPost
            {
                Content = "Life seemed to have stopped short; the shops were shut, the streets deserted.",
                Comments = new List<TwikkerComment> {comment6},
                PostDate = DateTime.Now,
                Owner = dichter
            };

            var post6 = new TwikkerPost
            {
                Content =
                    "Small detachments of soldiers knocked at each door, and then disappeared within the houses; for the vanquished saw they would have to be civil to their conquerors.",
                PostDate = DateTime.Now,
                Owner = huber
            };

            var post7 = new TwikkerPost
            {
                Content = "Even the town itself resumed by degrees its ordinary aspect.",
                Comments = new List<TwikkerComment> {comment7},
                PostDate = DateTime.Now,
                Owner = schneider
            };

            var post8 = new TwikkerPost
            {
                Content = "The French seldom walked abroad, but the streets swarmed with Prussian soldiers.",
                PostDate = DateTime.Now,
                Owner = dichter
            };
            var post9 = new TwikkerPost
            {
                Content =
                    "But there was something in the air, a something strange and subtle, an intolerable foreign atmosphere like a penetrating odor--the odor of invasion.",
                Comments = new List<TwikkerComment> {comment8, comment9, comment10},
                PostDate = DateTime.Now,
                Owner = bauer
            };
            var post10 = new TwikkerPost
            {
                Content = "The conquerors exacted money, much money.",
                PostDate = DateTime.Now,
                Owner = dichter
            };

            context.Posts.Add(post1);
            context.Posts.Add(post2);
            context.Posts.Add(post3);
            context.Posts.Add(post4);
            context.Posts.Add(post5);
            context.Posts.Add(post6);
            context.Posts.Add(post7);
            context.Posts.Add(post8);
            context.Posts.Add(post9);
            context.Posts.Add(post10);
            await context.SaveChangesAsync();
        }
    }
}