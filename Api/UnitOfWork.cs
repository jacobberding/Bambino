using Api.Models;
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

        private GenericRepository<ContactFile> contactFileRepository;
        public GenericRepository<ContactFile> ContactFileRepository { get { if (this.contactFileRepository == null) this.contactFileRepository = new GenericRepository<ContactFile>(context); return contactFileRepository; } }

        private GenericRepository<Discipline> disciplineRepository;
        public GenericRepository<Discipline> DisciplineRepository { get { if (this.disciplineRepository == null) this.disciplineRepository = new GenericRepository<Discipline>(context); return disciplineRepository; } }

        private GenericRepository<Email> emailRepository;
        public GenericRepository<Email> EmailRepository { get { if (this.emailRepository == null) this.emailRepository = new GenericRepository<Email>(context); return emailRepository; } }

        private GenericRepository<Log> logRepository;
        public GenericRepository<Log> LogRepository { get { if (this.logRepository == null) this.logRepository = new GenericRepository<Log>(context); return logRepository; } }

        private GenericRepository<Material> materialRepository;
        public GenericRepository<Material> MaterialRepository { get { if (this.materialRepository == null) this.materialRepository = new GenericRepository<Material>(context); return materialRepository; } }

        private GenericRepository<MaterialTag> materialTagRepository;
        public GenericRepository<MaterialTag> MaterialTagRepository { get { if (this.materialTagRepository == null) this.materialTagRepository = new GenericRepository<MaterialTag>(context); return materialTagRepository; } }

        private GenericRepository<MaterialPriceOption> materialPriceOptionRepository;
        public GenericRepository<MaterialPriceOption> MaterialPriceOptionRepository { get { if (this.materialPriceOptionRepository == null) this.materialPriceOptionRepository = new GenericRepository<MaterialPriceOption>(context); return materialPriceOptionRepository; } }

        private GenericRepository<Member> memberRepository;
        public GenericRepository<Member> MemberRepository { get { if (this.memberRepository == null) this.memberRepository = new GenericRepository<Member>(context); return memberRepository; } }

        private GenericRepository<MemberIpAddress> memberIpAddressRepository;
        public GenericRepository<MemberIpAddress> MemberIpAddressRepository { get { if (this.memberIpAddressRepository == null) this.memberIpAddressRepository = new GenericRepository<MemberIpAddress>(context); return memberIpAddressRepository; } }

        private GenericRepository<Project> projectRepository;
        public GenericRepository<Project> ProjectRepository { get { if (this.projectRepository == null) this.projectRepository = new GenericRepository<Project>(context); return projectRepository; } }

        private GenericRepository<ProjectPhase> projectPhaseRepository;
        public GenericRepository<ProjectPhase> ProjectPhaseRepository { get { if (this.projectPhaseRepository == null) this.projectPhaseRepository = new GenericRepository<ProjectPhase>(context); return projectPhaseRepository; } }

        private GenericRepository<ProjectZone> projectZoneRepository;
        public GenericRepository<ProjectZone> ProjectZoneRepository { get { if (this.projectZoneRepository == null) this.projectZoneRepository = new GenericRepository<ProjectZone>(context); return projectZoneRepository; } }

        private GenericRepository<ProjectAttraction> projectAttractionRepository;
        public GenericRepository<ProjectAttraction> ProjectAttractionRepository { get { if (this.projectAttractionRepository == null) this.projectAttractionRepository = new GenericRepository<ProjectAttraction>(context); return projectAttractionRepository; } }

        private GenericRepository<ProjectElement> projectElementRepository;
        public GenericRepository<ProjectElement> ProjectElementRepository { get { if (this.projectElementRepository == null) this.projectElementRepository = new GenericRepository<ProjectElement>(context); return projectElementRepository; } }

        private GenericRepository<ProjectWritingDocument> projectWritingDocumentRepository;
        public GenericRepository<ProjectWritingDocument> ProjectWritingDocumentRepository { get { if (this.projectWritingDocumentRepository == null) this.projectWritingDocumentRepository = new GenericRepository<ProjectWritingDocument>(context); return projectWritingDocumentRepository; } }
        
        private GenericRepository<Report> reportRepository;
        public GenericRepository<Report> ReportRepository { get { if (this.reportRepository == null) this.reportRepository = new GenericRepository<Report>(context); return reportRepository; } }

        private GenericRepository<Role> roleRepository;
        public GenericRepository<Role> RoleRepository { get { if (this.roleRepository == null) this.roleRepository = new GenericRepository<Role>(context); return roleRepository; } }

        private GenericRepository<Tsk> tskRepository;
        public GenericRepository<Tsk> TskRepository { get { if (this.tskRepository == null) this.tskRepository = new GenericRepository<Tsk>(context); return tskRepository; } }

        private GenericRepository<SubTsk> subTskRepository;
        public GenericRepository<SubTsk> SubTskRepository { get { if (this.subTskRepository == null) this.subTskRepository = new GenericRepository<SubTsk>(context); return subTskRepository; } }

        private GenericRepository<TimeTracker> timeTrackerRepository;
        public GenericRepository<TimeTracker> TimeTrackerRepository { get { if (this.timeTrackerRepository == null) this.timeTrackerRepository = new GenericRepository<TimeTracker>(context); return timeTrackerRepository; } }

        private GenericRepository<TimeTrackerProject> timeTrackerProjectRepository;
        public GenericRepository<TimeTrackerProject> TimeTrackerProjectRepository { get { if (this.timeTrackerProjectRepository == null) this.timeTrackerProjectRepository = new GenericRepository<TimeTrackerProject>(context); return timeTrackerProjectRepository; } }

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