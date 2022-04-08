using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreationModelPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreationModel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Level level1 = GetLevel(doc);
            CreateWall(doc, level1.Id, UnitUtils.ConvertToInternalUnits(5, UnitTypeId.Meters),
                UnitUtils.ConvertToInternalUnits(10, UnitTypeId.Meters),
                UnitUtils.ConvertToInternalUnits(3, UnitTypeId.Meters));


            return Result.Succeeded;
        }

        public Level GetLevel(Document doc)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Level level1 = listLevel
                .Where(x => x.Name.Equals("Уровень 1"))
                .FirstOrDefault();

            return level1;
        }

        public List<Wall> CreateWall(Document doc, ElementId levelId, double width, double depth, double height)
        {
            //UnitUtils.ConvertToInternalUnits(width, UnitTypeId.Meters);
            //UnitUtils.ConvertToInternalUnits(depth, UnitTypeId.Meters);
            //UnitUtils.ConvertToInternalUnits(height, UnitTypeId.Meters);

            double dx = width / 2;
            double dy = depth / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            List<WallType> listWallType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .OfType<WallType>()
                .ToList();

            WallType wall1 = listWallType
                .Where(x => x.Name.Equals("Наружный - Стена из кирпича с наружным слоем лицевого кирпича толщиной 380 мм и Кирпич фасадный - 250ммx65мм оштукатуренная 25 мм"))
                .FirstOrDefault();


            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Create wall");
            transaction.Start();
            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, wall1.Id, levelId, height, 0, false, false);
                walls.Add(wall);
            }

            transaction.Commit();

            return walls;
        }
    }
}
