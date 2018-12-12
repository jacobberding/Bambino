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

        private GenericRepository<Layer> layerRepository;
        public GenericRepository<Layer> LayerRepository { get { if (this.layerRepository == null) this.layerRepository = new GenericRepository<Layer>(context); return layerRepository; } }

        private GenericRepository<Category> categoryRepository;
        public GenericRepository<Category> CategoryRepository { get { if (this.categoryRepository == null) this.categoryRepository = new GenericRepository<Category>(context); return categoryRepository; } }

        private GenericRepository<Contact> contactRepository;
        public GenericRepository<Contact> ContactRepository { get { if (this.contactRepository == null) this.contactRepository = new GenericRepository<Contact>(context); return contactRepository; } }

        private GenericRepository<Discipline> disciplineRepository;
        public GenericRepository<Discipline> DisciplineRepository { get { if (this.disciplineRepository == null) this.disciplineRepository = new GenericRepository<Discipline>(context); return disciplineRepository; } }

        private GenericRepository<Project> projectRepository;
        public GenericRepository<Project> ProjectRepository { get { if (this.projectRepository == null) this.projectRepository = new GenericRepository<Project>(context); return projectRepository; } }

        private GenericRepository<Report> reportRepository;
        public GenericRepository<Report> ReportRepository { get { if (this.reportRepository == null) this.reportRepository = new GenericRepository<Report>(context); return reportRepository; } }

        private GenericRepository<TagAreaType> tagAreaTypeRepository;
        public GenericRepository<TagAreaType> TagAreaTypeRepository { get { if (this.tagAreaTypeRepository == null) this.tagAreaTypeRepository = new GenericRepository<TagAreaType>(context); return tagAreaTypeRepository; } }
        
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