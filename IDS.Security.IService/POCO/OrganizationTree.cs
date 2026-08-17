using IDS.Common;
using IDS.Security.Module;
using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService.POCO
{
    public class OrganizationTree: VOrganization
    {
        public List<OrganizationTree> children { set; get; }
        public static List<OrganizationTree> createAllOrganizationTree(List<VOrganization> organizationViews)
        {
            String pid = "#";
            List<VOrganization> pids = organizationViews;
            if (pids == null || pids.Count() == 0)
                throw new BussinessException("没有配置组织信息");
            List<OrganizationTree> organizationTrees = new List<OrganizationTree>();
            List<OrganizationTree> roots = organizationViews.Where(c=>c.DeptType == 100).Select(m=>{
                OrganizationTree organizationTree = new OrganizationTree();
                organizationTree.Name=m.Name;
                organizationTree.Code=m.Code;
                organizationTree.CreateUser=m.CreateUser;
                organizationTree.Grade=m.Grade;
                organizationTree.DeptType=m.DeptType;
                organizationTree.Id=m.Id;
                organizationTree.Pid=m.Pid;
                organizationTree.Sort=m.Sort;
                organizationTree.OrgId=m.OrgId;
                organizationTree.JobDsc=m.JobDsc;
                organizationTree.Status=m.Status;
                return organizationTree;
            }).ToList();
            organizationTrees.AddRange(roots);
            organizationTrees.ForEach(c=>{
                c.children=createOrganizationTree(organizationViews, c.Id);
                //organizationTrees.addAll(createOrganizationTree(organizationViews,c.getId);
            });
            return organizationTrees;
        }

        private static List<OrganizationTree> createOrganizationTree(List<VOrganization> organizationViews, String pid)
        {

            List<OrganizationTree> pids = organizationViews.Where(m=>m.Pid.Equals(pid)).Select(m=>{
                OrganizationTree organizationTree = new OrganizationTree();
                organizationTree.Name=m.Name;
                organizationTree.Code=m.Code;
                organizationTree.CreateUser=m.CreateUser;
                organizationTree.Grade=m.Grade;
                organizationTree.DeptType=m.DeptType;
                organizationTree.Id=m.Id;
                organizationTree.Pid=m.Pid;
                organizationTree.Sort=m.Sort;
                organizationTree.OrgId=m.OrgId;
                organizationTree.JobDsc=m.JobDsc;
                organizationTree.Status=m.Status;
                return organizationTree;
            }).ToList();
            foreach (OrganizationTree t in pids)
            {
                t.children=createOrganizationTree(organizationViews, t.Id);
            }
            return pids;
        }


        public static List<OrganizationTree> createFilterOrganizationTree(List<VOrganization> organizationViews)
        {
            String pid = "#";
            List<VOrganization> pids = organizationViews;
            if (pids == null || pids.Count() == 0)
                throw new BussinessException("没有配置组织信息");
            List<OrganizationTree> organizationTrees = new List<OrganizationTree>();
            List<OrganizationTree> roots = organizationViews.Where(c=>c.DeptType == 100).Select(m=>{
                OrganizationTree organizationTree = new OrganizationTree();
                organizationTree.Name=m.Name;
                organizationTree.Code=m.Code;
                organizationTree.CreateUser=m.CreateUser;
                organizationTree.Grade=m.Grade;
                organizationTree.DeptType=m.DeptType;
                organizationTree.Id=m.Id;
                organizationTree.Pid=m.Pid;
                organizationTree.Sort=m.Sort;
                organizationTree.OrgId=m.OrgId;
                organizationTree.JobDsc=m.JobDsc;
                organizationTree.Status=m.Status;
                return organizationTree;
            }).ToList();
            organizationTrees.AddRange(roots);
            organizationTrees.ForEach(c=>{
                c.children=createOrganizationTree(organizationViews, c.Id);
                //organizationTrees.addAll(createOrganizationTree(organizationViews,c.getId);
            });
            filterUserOrgTree(organizationTrees);
            return organizationTrees;
        }

        private static bool filterUserOrgTree(List<OrganizationTree> organizationTrees)
        {

            foreach (var organizationTree in organizationTrees) {

                if (organizationTree.children == null || organizationTree.children.Count() == 0)
                {
                    if (AuthConstant.USER!=organizationTree.DeptType)
                    {
                        organizationTrees.Remove(organizationTree);
                        continue;
                    }
                }
                if (organizationTree.children != null && organizationTree.children.Count() > 0)
                {
                    if (!filterUserOrgTree(organizationTree.children))
                    {
                        organizationTrees.Remove(organizationTree);
                    }
                }
            }
            return organizationTrees.Count() > 0;
        }
    }
    public class AuthConstant
    {
        public  const int ORG = 100;
      public const int DEPT = 200;
       public const int USER = 300;
}
}
