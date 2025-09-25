# 📝 Satisfactory Planner - Current Development Tasks

## 🎯 Current Status Summary - v0.5 Alpha
**🎉 MAJOR MILESTONE ACHIEVED** - Full visual production planner is complete and working!
- **Visual Interface**: ✅ Complete Avalonia UI with drag-and-drop functionality
- **Production Planning**: ✅ Multi-target support with flow graph visualization
- **Real-time Updates**: ✅ Dynamic connection lines and live calculations
- **Architecture**: ✅ Clean MVVM pattern with comprehensive data binding
- **User Experience**: ✅ Professional interface with debug console

---

## 🎨 Phase 6: Visual Enhancement Tasks (Next Sprint)

### **High Priority - Visual Features**

#### 1. **Building & Item Icons Integration** 🎨
- [ ] **ProductionNodeViewModel Enhancement**
  - Add BuildingIcon property for visual display
  - Integrate with existing IconService
  - Add fallback icons for missing assets
  - Cache icons for performance

- [ ] **Connection Line Item Icons**
  - Display item icons on connection lines
  - Show production rates and item names
  - Add hover tooltips with detailed information
  - Implement icon positioning along lines

#### 2. **Output Item Nodes** �
- [ ] **Create OutputItemNodeViewModel**
  - New node type for final production targets
  - Display target item icon and required quantity
  - Position at the end of production chains
  - Make multi-target graphs visually distinct

- [ ] **Update Flow Graph Layout**
  - Modify CalculateFlowGraphPositions to include output nodes
  - Extend connection lines to reach output nodes
  - Adjust spacing for better visual hierarchy

#### 3. **UI Polish & User Experience** ✨
- [ ] **Node Styling Improvements**
  - Enhanced visual design with better colors and shadows
  - Hover effects and selection states
  - Building type color coding (miners, smelters, constructors, etc.)
  - Better typography and layout within nodes

- [ ] **Interaction Enhancements**
  - Grid snap for precise positioning
  - Multi-select and group operations
  - Context menus for node operations
  - Keyboard shortcuts for common actions

---

### **Medium Priority - Data & Persistence**

#### 4. **Save/Load System Implementation** 💾
- [ ] **Production Plan Serialization**
  - Serialize complete production graphs with node positions
  - Include user customizations (node positions, layout preferences)
  - Version compatibility and migration handling
  - Auto-save functionality for crash recovery

#### 5. **User Preferences** ⚙️
- [ ] **Settings System**
  - Window size and position persistence
  - Panel visibility preferences (debug console, etc.)
  - Default production targets and common configurations
  - Theme and appearance customization

---

### **Low Priority - Advanced Features**

#### 6. **Production Analysis Tools** �
- [ ] **Bottleneck Detection**
  - Identify limiting factors in production chains
  - Visual indicators for underutilized buildings
  - Optimization suggestions and recommendations
  - Alternative recipe path analysis

#### 7. **Factory Layout Tools** 🏗️
- [ ] **Spatial Planning Features**
  - Grid snap and alignment tools
  - Building footprint visualization
  - Foundation planning helpers
  - Belt/pipe routing suggestions

#### 8. **Advanced Calculations** 🧮
- [ ] **Power Consumption Analysis**
  - Total power requirements for production chains
  - Power generation planning and optimization
  - Overclocking vs. building count analysis
  - Resource efficiency metrics and sustainability planning

---

## 🚀 Future Enhancement Vision

#### **Phase 7: Community & Sharing** (Future)
- [ ] **Plan Sharing System**
  - Export production plans as shareable files
  - Community repository for common production setups
  - Template gallery for popular factory configurations
  - Integration with Satisfactory community tools

- [ ] **Game Integration**
  - Validate data against latest game updates
  - Auto-update system for new game content
  - Integration with game save files (read-only analysis)
  - Export to game-compatible formats

---

## 📋 Current Sprint Focus

### ✅ **Completed This Week:**
- Interactive drag-and-drop production flow graphs
- Real-time connection lines that follow node movement
- Multi-target production chain support with proper separation
- Professional MVVM architecture with comprehensive data binding
- Debug console with toggle functionality
- Clean UI with proper flow-based node positioning

### 🎯 **Next Week Goals:**
- Integrate building and item icons into the visual interface
- Add dedicated output item nodes for clearer multi-target visualization
- Enhance UI styling and user experience polish
- Implement basic save/load functionality for production plans

### **Tomorrow's Session Start:**
1. ✅ Run `dotnet build` - verify still successful
2. ✅ Run `dotnet test` - verify all tests pass
3. ✅ Run console app - verify functionality
4. 🔍 Review current obsolete warnings with `get_errors()`
5. 🎯 Pick highest priority task from above list

### **Before Each Code Change:**
- 🧪 Run tests to ensure no regressions
- 📝 Update this TODO list with progress
- 💡 Note any new issues discovered

### **End of Session:**
- ✅ Verify build still successful
- ✅ Verify tests still pass  
- ✅ Update TODO list with completed items
- 📝 Add any new discoveries or tasks

---

## 🎉 What We've Accomplished So Far

### **✅ Phase 1: Foundation** 
- Created Building base class with wiki-aligned terminology
- Implemented Machine → Building inheritance with backward compatibility
- Created BuildingType enum matching wiki categories

### **✅ Phase 2: Core Models**
- Updated ProductionNode to use Building/BuildingCount properties
- Added obsolete Machine/MachineCount properties for backward compatibility
- Updated ProductionGraph models

### **✅ Phase 3: Services & Repositories**
- Migrated ProductionGraphBuilder to use IBuildingRepository
- Created SelectBestBuilding method alongside obsolete SelectBestMachine
- Updated all production planning logic to use Building terminology

### **✅ Phase 4: User Interface & Extensions**
- Updated console application output to show Building terminology
- Updated test files to use new Building methods
- Added Building extension methods with Machine obsolete alternatives
- Updated GameDataService with Building support

---

## 🔍 Key Files to Monitor
- `SatisfactoryPlanner.GameData/GameDataService.cs` - Remove MachineLoader dependency
- `SatisfactoryPlanner.GameData/Loaders/BuildingLoader.cs` - Native implementation
- `SatisfactoryPlanner.GameData/Demo/IconDemo.cs` - Update to Buildings
- Any files with remaining obsolete warnings

---

## 💡 Notes & Insights
- **Backward Compatibility Strategy Working Perfectly** - Zero breaking changes achieved
- **Inheritance Pattern Success** - Machine inheriting from Building allows seamless casting
- **Obsolete Attributes Effective** - Providing clear migration guidance to developers
- **Test Coverage Maintained** - All functionality preserved through migration
- **Wiki Alignment Goal Achieved** - Core terminology now matches official Satisfactory wiki

---

*Created: September 23, 2025*  
*Status: Migration 85% Complete - Core functionality migrated successfully*