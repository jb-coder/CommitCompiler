using CommitCompilerShared.Data;
using CommitCompilerShared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommitCompiler.Services
{
    public class BuildConfigurationService
    {
        private readonly CommitCompilerContext _context;

        public BuildConfigurationService(CommitCompilerContext context)
        {
            _context = context;
        }

        public void AddBuildConfiguration(BuildConfiguration config)
        {
            _context.BuildConfigurations.Add(config);
            _context.SaveChanges();
        }

        public List<BuildConfiguration> GetAllBuildConfigurations()
        {
            return _context.BuildConfigurations.ToList();
        }
        public async Task<List<BuildConfiguration>> GetAllConfigurationsAsync()
        {
            return await _context.BuildConfigurations.ToListAsync();
        }
        public BuildConfiguration GetBuildConfigurationById(int id)
        {
            return _context.BuildConfigurations.Find(id);
        }

        public void UpdateBuildConfiguration(BuildConfiguration config)
        {
            var existingConfig = _context.BuildConfigurations.Find(config.Id);
            if (existingConfig != null)
            {
                _context.Entry(existingConfig).CurrentValues.SetValues(config);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("BuildConfiguration not found");
            }
        }

        public void DeleteBuildConfiguration(int id)
        {
            var config = _context.BuildConfigurations.Find(id);
            if (config != null)
            {
                _context.BuildConfigurations.Remove(config);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("BuildConfiguration not found");
            }
        }
    }
}
