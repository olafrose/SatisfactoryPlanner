using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using SatisfactoryPlanner.GameData.Services;
using SatisfactoryPlanner.GameData.Models;
using SatisfactoryPlanner.GameData.Extensions;

namespace SatisfactoryPlanner.Avalonia.Services;

public class AvaloniaIconService
{
    private readonly IconService _iconService;
    
    public AvaloniaIconService(IconService iconService)
    {
        _iconService = iconService;
    }
    
    public async Task<Bitmap?> GetMilestoneIconAsync(Milestone milestone, bool isCompleted = false)
    {
        try
        {
            var iconPath = await milestone.GetIconPathAsync(_iconService);
            if (iconPath == null || !File.Exists(iconPath))
                return null;
                
            var bitmap = new Bitmap(iconPath);
            
            // For completed milestones, we could apply a filter or use a different icon
            // For now, just return the original bitmap
            // In the future, we could implement icon inversion here
            
            return bitmap;
        }
        catch (Exception)
        {
            // Return null if icon loading fails
            return null;
        }
    }
    
    public async Task<Bitmap?> GetItemIconAsync(Item item)
    {
        try
        {
            var iconPath = await item.GetIconPathAsync(_iconService);
            if (iconPath == null || !File.Exists(iconPath))
                return null;
                
            return new Bitmap(iconPath);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<Bitmap?> GetBuildingIconAsync(Building building)
    {
        try
        {
            var iconPath = await building.GetIconPathAsync(_iconService);
            if (iconPath == null || !File.Exists(iconPath))
                return null;
                
            return new Bitmap(iconPath);
        }
        catch (Exception)
        {
            return null;
        }
    }
}