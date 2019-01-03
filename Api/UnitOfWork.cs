﻿using Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Api
{
    public class UnitOfWork : IDisposable
    {
        private EDCContext context = new EDCContext();

        private GenericRepository<ACAreaCategory> acAreaCategoryRepository;
        public GenericRepository<ACAreaCategory> ACAreaCategoryRepository { get { if (this.acAreaCategoryRepository == null) this.acAreaCategoryRepository = new GenericRepository<ACAreaCategory>(context); return acAreaCategoryRepository; } }

        private GenericRepository<ACLayer> acLayerRepository;
        public GenericRepository<ACLayer> ACLayerRepository { get { if (this.acLayerRepository == null) this.acLayerRepository = new GenericRepository<ACLayer>(context); return acLayerRepository; } }

        private GenericRepository<ACLayerCategory> acLayerCategoryRepository;
        public GenericRepository<ACLayerCategory> ACLayerCategoryRepository { get { if (this.acLayerCategoryRepository == null) this.acLayerCategoryRepository = new GenericRepository<ACLayerCategory>(context); return acLayerCategoryRepository; } }

        private GenericRepository<ApiToken> apiTokenRepository;
        public GenericRepository<ApiToken> ApiTokenRepository { get { if (this.apiTokenRepository == null) this.apiTokenRepository = new GenericRepository<ApiToken>(context); return apiTokenRepository; } }

        private GenericRepository<Company> companyRepository;
        public GenericRepository<Company> CompanyRepository { get { if (this.companyRepository == null) this.companyRepository = new GenericRepository<Company>(context); return companyRepository; } }

        private GenericRepository<Contact> contactRepository;
        public GenericRepository<Contact> ContactRepository { get { if (this.contactRepository == null) this.contactRepository = new GenericRepository<Contact>(context); return contactRepository; } }

        private GenericRepository<Discipline> disciplineRepository;
        public GenericRepository<Discipline> DisciplineRepository { get { if (this.disciplineRepository == null) this.disciplineRepository = new GenericRepository<Discipline>(context); return disciplineRepository; } }

        private GenericRepository<Email> emailRepository;
        public GenericRepository<Email> EmailRepository { get { if (this.emailRepository == null) this.emailRepository = new GenericRepository<Email>(context); return emailRepository; } }

        private GenericRepository<Log> logRepository;
        public GenericRepository<Log> LogRepository { get { if (this.logRepository == null) this.logRepository = new GenericRepository<Log>(context); return logRepository; } }

        private GenericRepository<Material> materialRepository;
        public GenericRepository<Material> MaterialRepository { get { if (this.materialRepository == null) this.materialRepository = new GenericRepository<Material>(context); return materialRepository; } }

        private GenericRepository<Member> memberRepository;
        public GenericRepository<Member> MemberRepository { get { if (this.memberRepository == null) this.memberRepository = new GenericRepository<Member>(context); return memberRepository; } }

        private GenericRepository<MemberIpAddress> memberIpAddressRepository;
        public GenericRepository<MemberIpAddress> MemberIpAddressRepository { get { if (this.memberIpAddressRepository == null) this.memberIpAddressRepository = new GenericRepository<MemberIpAddress>(context); return memberIpAddressRepository; } }

        private GenericRepository<Project> projectRepository;
        public GenericRepository<Project> ProjectRepository { get { if (this.projectRepository == null) this.projectRepository = new GenericRepository<Project>(context); return projectRepository; } }

        private GenericRepository<Report> reportRepository;
        public GenericRepository<Report> ReportRepository { get { if (this.reportRepository == null) this.reportRepository = new GenericRepository<Report>(context); return reportRepository; } }

        private GenericRepository<Role> roleRepository;
        public GenericRepository<Role> RoleRepository { get { if (this.roleRepository == null) this.roleRepository = new GenericRepository<Role>(context); return roleRepository; } }

        public void Save()
        {
            context.SaveChanges();
        }
        
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}