﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Web.Security;
using PMTool.Repository;

    public class CodeFirstRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                return this.GetType().Assembly.GetName().Name.ToString();
            }
            set
            {
                this.ApplicationName = this.GetType().Assembly.GetName().Name.ToString();
            }
        }

        public override bool RoleExists(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (PMToolContext Context = new PMToolContext())
            {
                Role Role = null;
                Role = Context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                if (Role != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (PMToolContext Context = new PMToolContext())
            {
                UserProfile User = null;
                User = Context.UserProfiles.FirstOrDefault(Usr => Usr.UserName == username);
                if (User == null)
                {
                    return false;
                }
                Role Role = Context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                if (Role == null)
                {
                    return false;
                }
                return User.Roles.Contains(Role);
            }
        }

        public override string[] GetAllRoles()
        {
            using (PMToolContext Context = new PMToolContext())
            {
                return Context.Roles.Select(Rl => Rl.RoleName).ToArray();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }
            using (PMToolContext Context = new PMToolContext())
            {
                Role Role = null;
                Role = Context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                if (Role != null)
                {
                    return Role.Users.Select(Usr => Usr.UserName).ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            using (PMToolContext Context = new PMToolContext())
            {
                UserProfile User = null;
                User = Context.UserProfiles.FirstOrDefault(Usr => Usr.UserName == username);
                if (User != null)
                {
                    return User.Roles.Select(Rl => Rl.RoleName).ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }

            if (string.IsNullOrEmpty(usernameToMatch))
            {
                return null;
            }

            using (PMToolContext Context = new PMToolContext())
            {

                return (from Rl in Context.Roles from Usr in Rl.Users where Rl.RoleName == roleName && Usr.UserName.Contains(usernameToMatch) select Usr.Username).ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                using (PMToolContext Context = new PMToolContext())
                {
                    Role Role = null;
                    Role = Context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                    if (Role == null)
                    {
                        Role NewRole = new Role
                        {
                            RoleId = Guid.NewGuid(),
                            RoleName = roleName
                        };
                        Context.Roles.Add(NewRole);
                        Context.SaveChanges();
                    }
                }
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (PMToolContext Context = new PMToolContext())
            {
                Role Role = null;
                Role = Context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                if (Role == null)
                {
                    return false;
                }
                if (throwOnPopulatedRole)
                {
                    if (Role.Users.Any())
                    {
                        return false;
                    }
                }
                else
                {
                    Role.Users.Clear();
                }
                Context.Roles.Remove(Role);
                Context.SaveChanges();
                return true;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (PMToolContext Context = new PMToolContext())
            {
                List<UserProfile> Users = Context.UserProfiles.Where(Usr => usernames.Contains(Usr.Username)).ToList();
                List<Role> Roles = Context.Roles.Where(Rl => roleNames.Contains(Rl.RoleName)).ToList();
                foreach (UserProfile user in Users)
                {
                    foreach (Role role in Roles)
                    {
                        if (!user.Roles.Contains(role))
                        {
                            user.Roles.Add(role);
                        }
                    }
                }
                Context.SaveChanges();
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (PMToolContext Context = new PMToolContext())
            {
                foreach (String username in usernames)
                {
                    String us = username;
                    UserProfile user = Context.UserProfiles.FirstOrDefault(U => U.UserName == us);
                    if (user != null)
                    {
                        foreach (String roleName in roleNames)
                        {
                            String rl = roleName;
                            Role role = user.Roles.FirstOrDefault(R => R.RoleName == rl);
                            if (role != null)
                            {
                                user.Roles.Remove(role);
                            }
                        }
                    }
                }
                Context.SaveChanges();
            }
        }
    }