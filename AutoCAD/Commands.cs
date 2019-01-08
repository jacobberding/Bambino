using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Api.Controllers;
using Api.Models;
using Exception = System.Exception;

namespace EDCCommands
{
    public class Commands
    {
        //ATTSync - look into this
        public static List<ACAreaCategoryViewModel> acAreaCategoryViewModels = new List<ACAreaCategoryViewModel>();
        public static List<ListViewModel> disciplineViewModels = new List<ListViewModel>();

        //Architectural = 4
        //Decimal = 2
        //engirneering = 3
        //fractional = 5
        //scientific = 1

        [CommandMethod("EDC-ProjectGet")]
        public static void ProjectGet()
        {

            try
            {

                PromptStringOptions pso = new PromptStringOptions("\nEnter a project code: ");
                pso.AllowSpaces = false;
                PromptResult pr = Active.Editor.GetString(pso);

                ProjectViewModel project = ProjectController._GetByCode(pr.StringResult);

                if (project == null)
                    throw new Exception(string.Format("No project found with code {0}", pr.StringResult));

                Active.Editor.WriteMessage("\nName: {0} ", project.name);
                Active.Editor.WriteMessage("\nAddress: {0} ", project.addressLine1 + " " + project.addressLine2 + " " + project.city + " " + project.state + " " + project.zip + " " + project.country);

            }
            catch (Exception ex)
            {
                Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
            }

        }

        [CommandMethod("EDC-SymbolTrim")]
        public static void SymbolTrim()
        {
            
            Active.Database.UsingTransaction((tr) =>
            {

                try
                {

                    string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
                    //var blocks = ms.OfType<BlockTableRecord>(tr).Where(c => c.Name == "EDC-" + m.Substring(0, 1) + "-G-Symbol Title");
                    BlockTable bt = (BlockTable)tr.GetObject(Active.Database.BlockTableId, OpenMode.ForRead);
                    //BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);
                    //EDC-I-G-Symbol Title
                    
                    foreach (ObjectId id in bt)
                    {

                        BlockTableRecord btr = tr.GetObject(id, OpenMode.ForRead) as BlockTableRecord;
                        int w = 0;

                        if (btr.Name != "EDC-" + m.Substring(0, 1) + "-G-Symbol Title")
                            continue;

                        Active.Editor.WriteMessage(String.Format("\nId: {0} / Name: {1}", id, btr.Name));

                        ObjectIdCollection blockReferenceIds = (ObjectIdCollection)btr.GetBlockReferenceIds(true, true);
                        
                        foreach (ObjectId blockReferenceId in blockReferenceIds)
                        {

                            BlockReference blkRef = (BlockReference)blockReferenceId.GetObject(OpenMode.ForRead);
                            
                            foreach (ObjectId attRefId in blkRef.AttributeCollection)
                            {
                                AttributeReference attRef = (AttributeReference)attRefId.GetObject(OpenMode.ForRead);

                                //if (attRef.ty == "Title") {
                                //    w = attRef.WidthFactor
                                //}

                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }

            });

        }

        [CommandMethod("EDC-LayerGetByKeyword")]
        public static void LayerGetByKeyword()
        {

            try
            {

                PromptStringOptions pso = new PromptStringOptions("\nEnter a keyword: ");
                pso.AllowSpaces = false;
                PromptResult pr = Active.Editor.GetString(pso);
                string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();

                List<ACLayerViewModel> acLayers = ACLayerController._GetByKeyword(new ACLayerGetByKeywordViewModel() { keyword = pr.StringResult, measurement = m });

                if (acLayers == null || acLayers.Count == 0)
                    throw new Exception(string.Format("No layers found matching {0} in measurement {1}", pr.StringResult, m));

                Active.Editor.WriteMessage("\nFound {0} Matching Layer(s)", acLayers.Count.ToString());
                foreach (var acLayer in acLayers)
                    Active.Editor.WriteMessage("\nName: {0} / Description: {1}", acLayer.name, acLayer.description);

            }
            catch (Exception ex)
            {
                Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
            }

        }

        [CommandMethod("EDC-LayerUpload")]
        public static void LayerUpload()
        {

            Active.Database.UsingTransaction((tr) =>
            {

                try
                {

                    string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
                    LayerTable lt = (LayerTable)tr.GetObject(Active.Database.LayerTableId, OpenMode.ForRead);
                    List<ACLayerAddManyViewModel> ls = new List<ACLayerAddManyViewModel>();

                    PromptKeywordOptions pko = new PromptKeywordOptions("");
                    pko.Message = "\nUpload Layers?";
                    pko.Keywords.Add("Yes");
                    pko.Keywords.Add("No");
                    PromptResult pr = Active.Editor.GetKeywords(pko);

                    if (pr.StringResult == "No")
                        throw new Exception("\nCanceled");

                    PromptStringOptions pso = new PromptStringOptions("\nEnter Password");
                    pr = Active.Editor.GetString(pso);

                    if (pr.StringResult != "6656")
                        throw new Exception("\nInvalid Password");

                    foreach (ObjectId id in lt)
                    {

                        LayerTableRecord layer = (LayerTableRecord)tr.GetObject(id, OpenMode.ForWrite);
                        if (layer.IsDependent || layer.Name.Length <= 3 || layer.Name.Substring(0, 3) != "EDC")
                            continue;
                        //The error occurs on the line below no clue why
                        //byte[] pi = layer.Transparency.Alpha;

                        LinetypeTableRecord ltt = (LinetypeTableRecord)tr.GetObject(layer.LinetypeObjectId, OpenMode.ForRead) as LinetypeTableRecord;
                        ACLayerAddManyViewModel a = new ACLayerAddManyViewModel()
                        {
                            categoryValue = layer.Name.Split('-')[2],
                            code = layer.Name.Substring(8).ToUpper(),
                            description = layer.Description,
                            color = layer.Color.ColorNameForDisplay,
                            isPlottable = layer.IsPlottable,
                            lineWeight = layer.LineWeight.ToString(),
                            lineType = ltt.Name,
                            measurement = m,
                            transparency = 0
                        };

                        ls.Add(a);

                    }

                    Active.Editor.WriteMessage("\nFound {0} Layer(s)", ls.Count);
                    //LayerController._AddMany(ls);
                    foreach (var l in ls)
                        Active.Editor.WriteMessage("\nCode: {0} / Description: {1} / Line Type: {2}", l.code, l.description, l.lineType);

                }
                catch (Exception ex)
                {
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }

            });

        }

        [CommandMethod("EDC-LayerImport")]
        public static void LayerImport()
        {

            Active.Database.UsingTransaction((tr) =>
            {

                try
                {

                    string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
                    List<ACLayerCategoryViewModel> categories = ACLayerCategoryController._Get();
                    List<ACLayerViewModel> layers = new List<ACLayerViewModel>();
                    LayerTable lt = (LayerTable)tr.GetObject(Active.Database.LayerTableId, OpenMode.ForRead);
                    LinetypeTable ltt = (LinetypeTable)tr.GetObject(Active.Database.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                    
                    PromptKeywordOptions pko = new PromptKeywordOptions("");
                    pko.Message = "\nCategory";
                    pko.Keywords.Add("All");
                    foreach (var obj in categories)
                        pko.Keywords.Add(obj.name);

                    PromptResult pr = Active.Editor.GetKeywords(pko);
                    ACLayerCategoryViewModel category = (pr.StringResult == "All") ? new ACLayerCategoryViewModel() { name = "All" } : categories.Where(i => i.name.Contains(pr.StringResult)).FirstOrDefault();

                    layers = ACLayerController._GetByCategory(new ACLayerGetByCategoryViewModel() { category = category.name, measurement = m });

                    if (layers.Count == 0)
                        throw new Exception(String.Format("\nNo Layers in {0} Category for {1}", category.name, m));
                    else
                        Active.Editor.WriteMessage(String.Format("\nLayers Added from {0} Category", category.name));

                    foreach (var layer in layers)
                    {
                        if (!lt.Has(layer.name))
                        {

                            LayerTableRecord ltblRec = new LayerTableRecord();
                            ltblRec.Name = layer.name;
                            ltblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, GetColor(layer.color));
                            ltblRec.LineWeight = GetLineWeight(layer.lineWeight);
                            ltblRec.Description = layer.description;
                            ltblRec.IsPlottable = layer.isPlottable;

                            if (ltt.Has(layer.lineType))
                            {
                                ltblRec.LinetypeObjectId = ltt[layer.lineType];
                            }
                            else
                            {
                                Active.Database.LoadLineTypeFile(layer.lineType, "acad.lin");
                                ltblRec.LinetypeObjectId = ltt[layer.lineType];
                            }

                            lt.UpgradeOpen();
                            lt.Add(ltblRec);
                            tr.AddNewlyCreatedDBObject(ltblRec, true);

                            //Has to occur after the AddNewlyCreatedDBObject
                            if (layer.transparency > 0)
                            {
                                byte alpha = (byte)(255 * (100 - layer.transparency) / 100);
                                ltblRec.Transparency = new Transparency(alpha);
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }

            });

        }
        
        [CommandMethod("EDC-LayerClean")]
        public static void LayerClean()
        {

            //Active.Document.LockOrUnlockAllLayers(false);
            Active.Database.UsingModelSpace((tr, ms) =>
            {
                
                try
                {

                    LayerTable lt = (LayerTable)tr.GetObject(Active.Database.LayerTableId, OpenMode.ForRead);
                    LinetypeTable ltt = (LinetypeTable)tr.GetObject(Active.Database.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                    //List<LayerTable> layersMS = ms.OfType<LayerTable>(tr).Where(i => !i.IsDependent && i.Name.Length > 3 && i.Name.Substring(0, 3) == "EDC").ToList();
                    string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
                    Active.Editor.WriteMessage(String.Format("\nScale: {0}", m));
                    List<ACLayerViewModel> acLayers = new List<ACLayerViewModel>();

                    PromptKeywordOptions pko = new PromptKeywordOptions("");
                    pko.Message = "\nMerge Duplicate Layers?";
                    pko.Keywords.Add("Yes");
                    pko.Keywords.Add("No");
                    PromptResult pr = Active.Editor.GetKeywords(pko);

                    acLayers = ACLayerController._GetByCategory(new ACLayerGetByCategoryViewModel() { category = "All", measurement = m });
                    
                    foreach (ObjectId id in lt)
                    {
                        
                        LayerTableRecord layerMS = (LayerTableRecord)tr.GetObject(id, OpenMode.ForWrite);
                        if (layerMS.IsDependent || layerMS.Name.Length <= 3 || layerMS.Name.Substring(0, 3) != "EDC")
                            continue;
                        //Active.Editor.WriteMessage(String.Format("\n{0}", layerMS.Name));

                        if (layerMS.IsLocked)
                            layerMS.IsLocked = false;

                        if (acLayers.Any(i => i.name.ToUpper() == layerMS.Name.ToUpper()))
                        {

                            //Active.Editor.WriteMessage(String.Format("\n1"));
                            ACLayerViewModel acLayer = acLayers.Where(i => i.name.ToUpper() == layerMS.Name.ToUpper()).FirstOrDefault();

                            layerMS.Name = acLayer.name;
                            layerMS.Color = Color.FromColorIndex(ColorMethod.ByAci, GetColor(acLayer.color));
                            layerMS.LineWeight = GetLineWeight(acLayer.lineWeight);
                            layerMS.Description = acLayer.description;
                            layerMS.IsPlottable = acLayer.isPlottable;
                            byte alpha = (byte)(255 * (100 - acLayer.transparency) / 100);
                            layerMS.Transparency = new Transparency(alpha);

                            if (ltt.Has(acLayer.lineType))
                            {
                                layerMS.LinetypeObjectId = ltt[acLayer.lineType];
                            }
                            else
                            {
                                Active.Database.LoadLineTypeFile(acLayer.lineType, "acad.lin");
                                layerMS.LinetypeObjectId = ltt[acLayer.lineType];
                            }

                        }
                        else if (pr.StringResult == "Yes")
                        {
                            
                            ACLayerViewModel layer = acLayers.Where(i => i.acLayerId == GetScore(acLayers, layerMS)).FirstOrDefault();
                            string newLayerName = layer.name;

                            if (!lt.Has(newLayerName))
                            {

                                Active.Editor.WriteMessage("\nLayer not found.");

                                layerMS.Name = layer.name;
                                layerMS.Color = Color.FromColorIndex(ColorMethod.ByAci, GetColor(layer.color));
                                layerMS.LineWeight = GetLineWeight(layer.lineWeight);
                                layerMS.Description = layer.description;
                                layerMS.IsPlottable = layer.isPlottable;
                                byte alpha = (byte)(255 * (100 - layer.transparency) / 100);
                                layerMS.Transparency = new Transparency(alpha);

                                if (ltt.Has(layer.lineType))
                                {
                                    layerMS.LinetypeObjectId = ltt[layer.lineType];
                                }
                                else
                                {
                                    Active.Database.LoadLineTypeFile(layer.lineType, "acad.lin");
                                    layerMS.LinetypeObjectId = ltt[layer.lineType];
                                }

                            }
                            else
                            {

                                ObjectId lid = lt[newLayerName];
                                var entities = ms.OfType<Entity>(tr, OpenMode.ForWrite).Where(c => c.Layer == layerMS.Name);
                                foreach (Entity ent in entities)
                                    ent.LayerId = lid;

                                //layerMS.Erase();
                                
                                //int changedCount = 0;
                                //// We have the layer table open, so let's get the layer ID and use that
                                //foreach (ObjectId id in psr.Value.GetObjectIds())
                                //{
                                //    using (Transaction tr = Active.Database.TransactionManager.StartTransaction())
                                //    {
                                //        Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                                //        ent.LayerId = lid;
                                //    }
                                //    // Could also have used: ent.Layer = newLayerName but this way is more efficient and cleaner
                                //    changedCount++;
                                //}
                                //
                                //Active.Editor.WriteMessage("\nChanged {0} entit{1} from layer \"{2}\" to layer \"{3}\".", changedCount, changedCount == 1 ? "y" : "ies", layerMS.Name, newLayerName);

                            }

                        }
                        else
                        {

                            //Active.Editor.WriteMessage(String.Format("\n2"));
                            ACLayerViewModel layer = acLayers.Where(i => i.acLayerId == GetScore(acLayers, layerMS)).FirstOrDefault();
                            bool w = lt.Has(layer.name);
                            
                            layerMS.Name = (!w) ? layer.name : String.Format(layer.name + " {0}", id); //layersMS.Where(i => i.Name.Contains(layer.name)).Count()
                            //Active.Editor.WriteMessage(String.Format("\nName Set {0}", l.name));
                            layerMS.Color = Color.FromColorIndex(ColorMethod.ByAci, GetColor(layer.color));
                            //Active.Editor.WriteMessage(String.Format("\nColor Set {0}", l.color));
                            layerMS.LineWeight = GetLineWeight(layer.lineWeight);
                            //Active.Editor.WriteMessage(String.Format("\nLineweight Set {0}", l.lineWeight));
                            layerMS.Description = layer.description;
                            //Active.Editor.WriteMessage(String.Format("\nDescription Set {0}", l.description));
                            layerMS.IsPlottable = layer.isPlottable;
                            //Active.Editor.WriteMessage(String.Format("\nPlottable Set {0}", l.isPlottable));
                            byte alpha = (byte)(255 * (100 - layer.transparency) / 100);
                            layerMS.Transparency = new Transparency(alpha);
                            //Active.Editor.WriteMessage(String.Format("\nTransparency Set {0}", l.transparency));

                            if (ltt.Has(layer.lineType))
                            {
                                layerMS.LinetypeObjectId = ltt[layer.lineType];
                            }
                            else
                            {
                                Active.Database.LoadLineTypeFile(layer.lineType, "acad.lin");
                                layerMS.LinetypeObjectId = ltt[layer.lineType];
                            }

                        }

                    }

                    Active.Editor.WriteMessage(String.Format("\nLayers Cleaned Successfully"));

                }
                catch (Exception ex)
                {
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }

            });

        }

        public static Guid GetScore(List<ACLayerViewModel> layers, LayerTableRecord layerMS)
        {

            Guid acLayerId = layers.Select(i => i.acLayerId).FirstOrDefault();
            double globalScore = 0;

            foreach (var layer in layers)
            {

                double score = 0;
                double scoreName = CalculateSimilarity(layer.name, layerMS.Name);
                double scoreDescription = CalculateSimilarity(layer.description, layerMS.Description);
                double scoreColor = (layer.color == layerMS.Color.ColorNameForDisplay) ? .25 : 0;
                double scoreLineWeight = (layer.lineWeight == layerMS.LineWeight.ToString()) ? .25 : 0;

                foreach (var e in layer.name.Substring(8).ToUpper().Split('-'))
                    if (layerMS.Name.ToUpper().Contains(e))
                        scoreName += 1;

                //best score 202
                score = scoreName + scoreDescription + scoreColor + scoreLineWeight;

                if (score > globalScore)
                {
                    globalScore = score;
                    acLayerId = layer.acLayerId;
                }

            }

            return acLayerId;

        }

        /// <summary>
        /// Calculate percentage similarity of two strings
        /// <param name="source">Source String to Compare with</param>
        /// <param name="target">Targeted String to Compare</param>
        /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
        /// </summary>
        public static double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }

        public static int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        public static double LongestCommonSubsequence(string s1, string s2)
        {

            int[,] num = new int[s1.Length, s2.Length];  //2D array
            char letter1;
            char letter2;

            //Actual algorithm
            for (int i = 0; i < s1.Length; i++)
            {
                letter1 = s1[i];
                for (int j = 0; j < s2.Length; j++)
                {
                    letter2 = s2[j];

                    if (letter1 == letter2)
                    {
                        if ((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];
                    }
                    else
                    {
                        if ((i == 0) && (j == 0))
                            num[i, j] = 0;
                        else if ((i == 0) && !(j == 0))   //First ith element
                            num[i, j] = Math.Max(0, num[i, j - 1]);
                        else if (!(i == 0) && (j == 0))   //First jth element
                            num[i, j] = Math.Max(num[i - 1, j], 0);
                        else // if (!(i == 0) && !(j == 0))
                            num[i, j] = Math.Max(num[i - 1, j], num[i, j - 1]);
                    }
                }//end j
            }//end i
            return (s2.Length - (double)num[s1.Length - 1, s2.Length - 1]) / s1.Length * 100;

        }

        public static LineWeight GetLineWeight(string lineWeight)
        {

            switch (lineWeight)
            {
                case "ByBlock":
                    return LineWeight.ByBlock;
                case "ByLayer":
                    return LineWeight.ByLayer;
                case "ByLineWeightDefault":
                    return LineWeight.ByLineWeightDefault;
                case "LineWeight000":
                    return LineWeight.LineWeight000;
                case "LineWeight005":
                    return LineWeight.LineWeight005;
                case "LineWeight009":
                    return LineWeight.LineWeight009;
                case "LineWeight013":
                    return LineWeight.LineWeight013;
                case "LineWeight015":
                    return LineWeight.LineWeight015;
                case "LineWeight018":
                    return LineWeight.LineWeight018;
                case "LineWeight020":
                    return LineWeight.LineWeight020;
                case "LineWeight025":
                    return LineWeight.LineWeight025;
                case "LineWeight030":
                    return LineWeight.LineWeight030;
                case "LineWeight035":
                    return LineWeight.LineWeight035;
                case "LineWeight040":
                    return LineWeight.LineWeight040;
                case "LineWeight050":
                    return LineWeight.LineWeight050;
                case "LineWeight053":
                    return LineWeight.LineWeight053;
                case "LineWeight060":
                    return LineWeight.LineWeight060;
                case "LineWeight070":
                    return LineWeight.LineWeight070;
                case "LineWeight080":
                    return LineWeight.LineWeight080;
                case "LineWeight090":
                    return LineWeight.LineWeight090;
                case "LineWeight100":
                    return LineWeight.LineWeight100;
                case "LineWeight106":
                    return LineWeight.LineWeight106;
                case "LineWeight120":
                    return LineWeight.LineWeight120;
                case "LineWeight140":
                    return LineWeight.LineWeight140;
                case "LineWeight158":
                    return LineWeight.LineWeight158;
                case "LineWeight200":
                    return LineWeight.LineWeight200;
                case "LineWeight211":
                    return LineWeight.LineWeight211;
                default:
                    return LineWeight.ByLineWeightDefault;
            }

        }

        public static short GetColor(string color)
        {

            int value;
            if (int.TryParse(color, out value))
                return Convert.ToInt16(color);

            switch (color)
            {
                case "red":
                    return 1;
                case "yellow":
                    return 2;
                case "green":
                    return 3;
                case "cyan":
                    return 4;
                case "blue":
                    return 5;
                case "magenta":
                    return 6;
                case "white":
                    return 7;
                default:
                    return 1;
            }

        }

        [CommandMethod("EDC-UnitsGet")]
        public void UnitsGet()
        {

            Active.Database.UsingTransaction((tr) =>
            {
                Active.Editor.WriteMessage("\nMeasurement : " + Active.Database.Measurement.ToString());
                Active.Editor.WriteMessage("\nUnits : " + Active.Database.Lunits.ToString());
            });

        }

        [CommandMethod("EDC-AreaTag")]
        public void AreaTag()
        {

            Active.Database.UsingTransaction((tr) =>
            {

                try
                {

                    //Prompt user to select a polyline
                    PromptEntityOptions peo = new PromptEntityOptions("\nSelect a polyline: ");
                    peo.SetRejectMessage("\nNot a polyline try again: ");
                    peo.AddAllowedClass(typeof(Polyline), true);

                    //Get the result
                    PromptEntityResult per;
                    per = Active.Editor.GetEntity(peo);

                    //Find the polyline
                    Polyline p = (Polyline)tr.GetObject(per.ObjectId, OpenMode.ForRead);

                    AreaTagDraw(tr, p);

                }
                catch (Exception ex)
                {
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }

            });

        }

        [CommandMethod("EDC-AreaTagAll")]
        public void AreaTagAll()
        {

            Active.Database.UsingModelSpace((tr, ms) =>
            {
                
                try
                {

                    bool isLayers = false;
                    string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
                    string[] layers = ACLayerController._GetByArea(m);
                    string layer = AddPromptKeywordOptions(Active.Document, "Select a type: ", layers, layers.FirstOrDefault());
                    IEnumerable<Polyline> polylines = ms.OfType<Polyline>(tr).Where(c => c.Layer == layer);

                    foreach (var p in polylines)
                    {
                        isLayers = true;
                        p.Highlight();
                        AreaTagDraw(tr, p);
                        p.Unhighlight();
                    }

                    if (!isLayers)
                        throw new Exception(String.Format("\nThere are no polylines on layer {0}", layer));

                }
                catch (Exception ex)
                {
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }

            });

        }

        public static void AreaTagDraw(Transaction tr, Polyline p)
        {
            
            // Get the block table from the drawing
            BlockTable bt = Active.Database.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
            BlockTableRecord ms = bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite) as BlockTableRecord;

            string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
            string blkName = "EDC-" + m.Substring(0, 1) + "-G-AreaTag";
            // Only set the block name if it isn't in use
            if (!bt.Has(blkName))
                throw new Exception(String.Format("\n{0} block does not exist please add it using EDC-AreaTagAddBlock", blkName));

            BlockTableRecord blockDef = bt[blkName].GetObject(OpenMode.ForRead) as BlockTableRecord;

            double area = p.Area;
            Field f = new Field();

            Active.Editor.WriteMessage("Area is {0}: ", area);
            //Point3d point = new Point3d(p.StartPoint.X, p.StartPoint.Y, p.StartPoint.Z);
            Point3d center = p.GeometricExtents.MinPoint + (p.GeometricExtents.MaxPoint - p.GeometricExtents.MinPoint) / 2.0;

            //AreaTagTypeController._GetByPage(new Search() { page = 1 });
            PromptStringOptions pso = new PromptStringOptions("\nEnter new area name: ");
            pso.AllowSpaces = true;
            PromptResult pr = Active.Editor.GetString(pso);
            // A variable for the block's name
            string name = pr.StringResult;

            if (acAreaCategoryViewModels.Count == 0)
                acAreaCategoryViewModels = ACAreaCategoryController._Get();

            string type = AddPromptKeywordOptions(Active.Document, "Select a type: ", acAreaCategoryViewModels.Select(i => i.name).ToArray(), acAreaCategoryViewModels.Select(i => i.name).FirstOrDefault());

            using (BlockReference blockRef = new BlockReference(center, blockDef.ObjectId))
            {

                //Add the block reference to modelspace
                ms.AppendEntity(blockRef);
                tr.AddNewlyCreatedDBObject(blockRef, true);

                //Iterate block definition to find all non-constant AttributeDefinitions
                foreach (ObjectId id in blockDef)
                {

                    DBObject obj = id.GetObject(OpenMode.ForRead);
                    AttributeDefinition attDef = obj as AttributeDefinition;

                    if ((attDef != null) && (!attDef.Constant))
                    {
                        Active.Editor.WriteMessage("\nBlocks is {0}: ", attDef.Tag);

                        //This is a non-constant AttributeDefinition
                        //Create a new AttributeReference
                        using (AttributeReference attRef = new AttributeReference())
                        {
                            //Add the AttributeReference to the BlockReference
                            blockRef.AttributeCollection.AppendAttribute(attRef);
                            tr.AddNewlyCreatedDBObject(attRef, true);
                            attRef.SetAttributeFromBlock(attDef, blockRef.BlockTransform);

                            if (attRef.Tag == "AREA")
                            {

                                Active.Editor.WriteMessage("\nPolyline id {0}: ", p.ObjectId);
                                Active.Editor.WriteMessage("\nPolyline expression {0}: ", GetAreaFields(attRef.Tag, p.ObjectId.ToString()));
                                Field fld = new Field(GetAreaFields(attRef.Tag, p.ObjectId.ToString()), true);
                                fld.SetFieldCode(GetAreaFields(attRef.Tag, p.ObjectId.ToString()));
                                fld.Evaluate();
                                FieldEvaluationStatusResult fieldEval = fld.EvaluationStatus;

                                if (fieldEval.Status != FieldEvaluationStatus.Success)
                                {
                                    tr.Abort();
                                    Active.Editor.WriteMessage(string.Format("\nFieldEvaluationStatus Message: {0} - {1}",
                                        fieldEval.Status, fieldEval.ErrorMessage));
                                    return;
                                }

                                try
                                {   //set the field to attribute reference 
                                    attRef.SetField(fld);
                                    tr.AddNewlyCreatedDBObject(fld, true);
                                    Active.Editor.WriteMessage(string.Format("\nField value ({1}) is inseterd to Attribute '{0}' of the Block '{2}' ",
                                        attRef.Tag, fld.Value, blockRef.Name));
                                }
                                catch
                                {
                                    fld.Dispose();
                                    Active.Editor.WriteMessage(string.Format("\nFailed to set attribute field '{0}' - {1}",
                                        attRef.Tag, attRef.Handle));
                                }

                                //tr.AddNewlyCreatedDBObject(fld, true);
                                //fld.
                                //attRef.TextString = GetAreaFields(attRef.Tag, p);

                            }
                            else if (attRef.Tag == "TYPE")
                            {
                                attRef.TextString = type;
                            }
                            else
                            {

                                attRef.TextString = name;

                            }

                        }
                    }

                }

            }

            // Report what we've done
            //ed.WriteMessage("\nCreated block named \"{0}\" containing {1} entities.", blkName, ents.Count);

        }

        protected static string AddPromptKeywordOptions(Document doc, string question, string[] words, string defaultWord)
        {
            PromptKeywordOptions pko = new PromptKeywordOptions("");
            pko.Message = question;
            pko.AllowNone = false;
            foreach (string word in words)
            {
                pko.Keywords.Add(word);
            }
            pko.Keywords.Default = defaultWord;
            //[color=red] pko.AllowArbitraryInput = true;[/color]

            PromptResult pr = doc.Editor.GetKeywords(pko);
            if (pr.Status != PromptStatus.OK) {
                return String.Empty;
            }
            return pr.StringResult;
        }

        private static string GetAreaFields(string layer, string objId)
        {
            
            string id = objId.Trim(new char[] { '(', ')' });

            if (layer == "AREA" && Active.Database.Lunits == 4) //Architectural
                return @"%<\AcObjProp.16.2 Object(%<\_ObjId " + id + @">%,1).Area \f ""%lu2%ct4%qf1%pr0 SQ. FT.%th44"">% ";
            else if (layer == "AREA" && Active.Database.Lunits == 2) //Decimal
                return @"%<\AcObjProp.16.2 Object(%<\_ObjId " + id + @">%,1).Area \f ""%lu2%pr1%ps[, S.M.]%ct8[1.000000000000000E-006]%th44"">% ";
            // DO NOT TOUCH WHAT IS ABOVE THE SPACING AND NUMBERS HAVE TO BE EXACT
            //%<\AcObjProp.16.2 Object(%<\_ObjId 3117022546384>%,1).Area \f "%lu2%pr0%ps[, SQ. FT.]%ct8[0.0069444444444444]%th44">%
            //%<\AcObjProp.16.2 Object(%<\_ObjId 22616372658>%,1).Area \f " %lu2%ct4%qf1%pr0 SQ. FT.%ct8[0.0069444444444444]">%
            //%<\AcObjProp.16.2 Object(%<\_ObjId 2088052963696                  >%,1).Area \f " %lu2 ">%
            //"%<\AcObjProp.16.2 Object(%<\_ObjId 2939429058208                  >%,1).Area \f " % lu2 % ps[, SQ.FT.] % ct8[0.0069444444444444]">%"
            //else if (pl.Layer == "AREA-AC")
            //    AStr = string.Format(@"%<\AcObjProp Object(%<\_ObjId  " + pl.ObjectId.ToString + @" \f ""%lu2%ct4%qf1 SQ. FT."">%", AStr);
            //else if (pl.Layer == "AREA-nonAC")
            //    NStr = string.Format(@"%<\AcObjProp Object(%<\_ObjId  " + pl.ObjectId.ToString + @" \f ""%lu2%ct4%qf1 SQ. FT."">%", NStr);

            return "";

        }

        private static decimal GetConversionRate()
        {

            string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
            decimal ret = 1;

            //12 inches = 305 mm
            if (m == "Metric") //Decimal
                ret = 305.0m / 12.0m;

            return ret;

        }

        [CommandMethod("EDC-AreaTagAddBlock")]
        public void AreaTagAddBlock()
        {

            Active.Database.UsingTransaction((tr) =>
            {

                string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
                string blkName = "EDC-" + m.Substring(0,1) + "-G-AreaTag";
                //var tables = ms.OfType<Table>(tr).Where(c => c.Layer == "EDC-G-Anno-Area");
                BlockTable bt = (BlockTable)tr.GetObject(Active.Database.BlockTableId, OpenMode.ForRead);
                
                try
                {
                    // Validate the provided symbol table name
                    SymbolUtilityServices.ValidateSymbolName(blkName, false);

                    // Only set the block name if it isn't in use
                    if (bt.Has(blkName))
                        throw new Exception(String.Format("\n{0} block with this name already exists.", blkName));

                    // Create our new block table record...
                    BlockTableRecord btr = new BlockTableRecord();

                    // ... and set its properties
                    btr.Name = blkName;

                    // Add the new block to the block table
                    bt.UpgradeOpen();
                    ObjectId btrId = bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);

                    LayerTable lt = (LayerTable)tr.GetObject(Active.Database.LayerTableId, OpenMode.ForRead);
                    List<ACLayerViewModel> acLayers = ACLayerController._GetAreaTag(m);

                    foreach (var acLayer in acLayers)
                    {
                        if (!lt.Has(acLayer.name))
                        {

                            LayerTableRecord ltblRec = new LayerTableRecord();

                            ltblRec.Name = acLayer.name;
                            ltblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, GetColor(acLayer.color));
                            ltblRec.LineWeight = GetLineWeight(acLayer.lineWeight);
                            ltblRec.Description = acLayer.description;
                            ltblRec.IsPlottable = acLayer.isPlottable;

                            lt.UpgradeOpen();
                            lt.Add(ltblRec);
                            tr.AddNewlyCreatedDBObject(ltblRec, true);

                            //Has to occur after the AddNewlyCreatedDBObject
                            if (acLayer.transparency > 0)
                            {
                                byte alpha = (byte)(255 * (100 - acLayer.transparency) / 100);
                                ltblRec.Transparency = new Transparency(alpha);
                            }

                        }
                    }
                    
                    //Add entities to block ---------------------------------------------------------------------
                    AttributeDefinition acAttDef = new AttributeDefinition();
                    acAttDef.Justify = AttachmentPoint.MiddleCenter;
                    acAttDef.AlignmentPoint = new Point3d(0, (double)(12 * GetConversionRate()), 0);
                    acAttDef.Verifiable = true;
                    acAttDef.Prompt = "NAME: ";
                    acAttDef.Tag = "NAME";
                    acAttDef.TextString = "NAME";
                    acAttDef.IsMTextAttributeDefinition = true;
                    acAttDef.MTextAttributeDefinition.UseBackgroundColor = true;
                    acAttDef.MTextAttributeDefinition.BackgroundScaleFactor = 1.25;
                    acAttDef.Layer = "EDC-I-G-NOTE";
                    acAttDef.Height = (double)(12 * GetConversionRate());
                    btr.AppendEntity(acAttDef);
                    tr.AddNewlyCreatedDBObject(acAttDef, true);

                    acAttDef = new AttributeDefinition();
                    acAttDef.Justify = AttachmentPoint.MiddleCenter;
                    acAttDef.AlignmentPoint = new Point3d(0, (double)(-48 * GetConversionRate()), 0);
                    acAttDef.Verifiable = true;
                    acAttDef.Prompt = "TYPE: ";
                    acAttDef.Tag = "TYPE";
                    acAttDef.TextString = "TYPE";
                    acAttDef.IsMTextAttributeDefinition = false;
                    acAttDef.Layer = "EDC-I-G-NOTE";
                    acAttDef.Invisible = true;
                    acAttDef.Height = (double)(12 * GetConversionRate());
                    btr.AppendEntity(acAttDef);
                    tr.AddNewlyCreatedDBObject(acAttDef, true);

                    acAttDef = new AttributeDefinition();
                    acAttDef.Justify = AttachmentPoint.MiddleCenter;
                    acAttDef.AlignmentPoint = new Point3d(0, (double)(-12 * GetConversionRate()), 0);
                    acAttDef.Verifiable = true;
                    acAttDef.Prompt = "AREA: ";
                    acAttDef.Tag = "AREA";
                    acAttDef.TextString = "AREA";
                    acAttDef.Layer = "EDC-I-G-AREA-NOTE";
                    acAttDef.Height = (double)(12 * GetConversionRate());

                    string fldCode = GetAreaFields("AREA", "0");
                    Field fld = new Field(fldCode, true);
                    fld.SetFieldCode(fldCode);
                    fld.Evaluate();
                    FieldEvaluationStatusResult fieldEval = fld.EvaluationStatus;

                    if (fieldEval.Status != FieldEvaluationStatus.Success)
                    {
                        tr.Abort();
                        throw new Exception(string.Format("\nFieldEvaluationStatus Message: {0} - {1}", fieldEval.Status, fieldEval.ErrorMessage));
                    }

                    //tr.AddNewlyCreatedDBObject(fld, true);
                    acAttDef.SetField(fld);

                    btr.AppendEntity(acAttDef);
                    tr.AddNewlyCreatedDBObject(acAttDef, true);
                    //Add entities to block ---------------------------------------------------------------------

                    // Report what we've done
                    Active.Editor.WriteMessage("\nCreated block named {0} containing 3 entities.", blkName);

                }
                catch (Exception ex)
                {
                    // An exception has been thrown, indicating the name is invalid
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }
                
            });
            
        }

        private DBObjectCollection SquareOfLines(double size)
        {

            // A function to generate a set of entities for our block
            DBObjectCollection ents = new DBObjectCollection();

            Point3d[] pts = { new Point3d(-size, -size, 0),new Point3d(size, -size, 0),new Point3d(size, size, 0),new Point3d(-size, size, 0) };

            int max = pts.GetUpperBound(0);

            for (int i = 0; i <= max; i++)
            {
                int j = (i == max ? 0 : i + 1);
                Line ln = new Line(pts[i], pts[j]);
                ents.Add(ln);
            }

            return ents;

        }

        [CommandMethod("EDC-AreaTable")]
        public static void AreaTable()
        {

            Active.Database.UsingTransaction((tr) =>
            {

                //Setup prompt options
                var promptOptions = new PromptStringOptions("\nEnter Title: ") { AllowSpaces = true };
                var result = Active.Editor.GetString(promptOptions);
                if (result.Status == PromptStatus.Cancel)
                    return;

                //Pick an insertion point
                PromptPointOptions pointOptions = new PromptPointOptions("Select insertion point: ");
                PromptPointResult pointResult = Active.Editor.GetPoint(pointOptions);
                if (result.Status == PromptStatus.Cancel)
                    return;

                AreaTableDraw(tr, Active.Editor, Active.Database, pointResult.Value, result.StringResult);
                
            });

        }

        [CommandMethod("EDC-AreaTableRegen")]
        public static void AreaTableRegen()
        {

            Active.Database.UsingModelSpace((tr, ms) =>
            {
                
                var tables = ms.OfType<Table>(tr).Where(c => c.Layer == "EDC-I-G-AREA-NOTE");
                Table table = (Table)tr.GetObject(tables.FirstOrDefault().ObjectId, OpenMode.ForWrite);

                AreaTableDraw(tr, Active.Editor, Active.Database, table.Position, table.Cells[0,0].TextString);

                //table.Dispose();
                table.Erase(true);
                //tr.Commit();

            });
            
        }
        
        public class attribute
        {
            public string name { get; set; }
            public string type { get; set; }
            public decimal area { get; set; }
        }
        
        public static void AreaTableDraw(Transaction tr, Editor ed, Database db, Point3d point, string tableName)
        {

            string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
            string blkName = "EDC-" + m.Substring(0, 1) + "-G-AreaTag";

            BlockTable blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            BlockTableRecord ms = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
            BlockTableRecord blockTableRecord = (BlockTableRecord)blockTable[blkName].GetObject(OpenMode.ForRead);
            ObjectIdCollection blockReferenceIds = (ObjectIdCollection)blockTableRecord.GetBlockReferenceIds(true, true);
            ed.WriteMessage(blockReferenceIds.Count.ToString());
            
            int columns = 1;
            List<attribute> attributes = new List<attribute>();
            int typeCount = 0;
            Color blue = Color.FromRgb(173,221,247);
            Color red = Color.FromRgb(242,113,114);

            foreach (ObjectId blockReferenceId in blockReferenceIds)
            {
                BlockReference blkRef = (BlockReference)blockReferenceId.GetObject(OpenMode.ForRead);

                attribute a = new attribute();

                foreach (ObjectId attRefId in blkRef.AttributeCollection)
                {
                    AttributeReference attRef = (AttributeReference)attRefId.GetObject(OpenMode.ForRead);

                    if (attRef.Tag == "NAME")
                        a.name = attRef.TextString;
                    else if (attRef.Tag == "TYPE")
                        a.type = attRef.TextString;
                    else if (attRef.Tag == "AREA")
                        a.area = GetIntFromString(attRef.TextString);

                    if (Active.Database.Lunits == 2)
                        a.area = a.area / 10;

                }

                attributes.Add(a);

            }

            attributes = attributes.OrderBy(i => i.type).ToList();
            typeCount = attributes.Select(i => i.type).Distinct().ToList().Count();
            columns += typeCount;

            Table tbl = new Table();
            int headerRows = 3;
            int footerRows = 2;
            int bottomRow = headerRows + attributes.Count() + (attributes.Select(i => i.type).Distinct().ToList().Count * 2) + 1;
            int textHeight = (int)(18 * GetConversionRate());
            int rowHeight = (int)(50 * GetConversionRate());
            tbl.Layer = "EDC-I-G-AREA-NOTE";
            tbl.SetDatabaseDefaults();
            tbl.Position = point;
            tbl.SetSize(headerRows + attributes.Count() + (attributes.Select(i => i.type).Distinct().ToList().Count * 2) + footerRows, columns);
            tbl.SetColumnWidth((int)(250 * GetConversionRate()));
            tbl.SetRowHeight(rowHeight);

            CellRange mcells = CellRange.Create(tbl, 0, 0, bottomRow, 2);
            mcells.Borders.Horizontal.Margin = rowHeight * 0.25;
            mcells.Borders.Top.LineWeight = LineWeight.LineWeight000;
            mcells.Borders.Bottom.LineWeight = LineWeight.LineWeight000;
            mcells.Borders.Left.LineWeight = LineWeight.LineWeight000;
            mcells.Borders.Right.LineWeight = LineWeight.LineWeight000;

            tbl.Cells[0, 0].SetValue(tableName, ParseOption.SetDefaultFormat);
            tbl.Cells[0, 0].TextHeight = textHeight;
            tbl.Cells[0, 0].Style = "";
            tbl.Cells[1, 0].SetValue("NAME", ParseOption.SetDefaultFormat);
            tbl.Cells[1, 0].TextHeight = textHeight;

            int rowIterator = headerRows + 1;
            int cellIterator = 1;
            foreach (var type in attributes.Select(i => i.type).Distinct())
            {
                tbl.Cells[1, cellIterator].SetValue(type + " AREA", ParseOption.SetDefaultFormat);
                tbl.Cells[1, cellIterator].TextHeight = textHeight;
                cellIterator++;
            }

            cellIterator = 1;
            string curType = attributes.FirstOrDefault().type;
            string append = (Active.Database.Lunits == 2) ? "S.M." : "SQ. FT.";
            string areaToString = (Active.Database.Lunits == 2) ? "#,##0.0" : "#,##0";
            foreach (var a in attributes)
            {
                
                if (curType != a.type)
                {
                    rowIterator += 2;
                    cellIterator++;
                    curType = a.type;
                }

                tbl.Cells[rowIterator, 0].SetValue(a.name, ParseOption.ParseOptionNone);
                tbl.Cells[rowIterator, 0].Alignment = CellAlignment.MiddleCenter;
                tbl.Cells[rowIterator, 0].TextHeight = textHeight;
                
                tbl.Cells[rowIterator, cellIterator].SetValue(a.area.ToString(areaToString) + " " + append, ParseOption.ParseOptionNone);
                tbl.Cells[rowIterator, cellIterator].Alignment = CellAlignment.MiddleCenter;
                tbl.Cells[rowIterator, cellIterator].TextHeight = textHeight;

                mcells = CellRange.Create(tbl, rowIterator, 1, rowIterator, columns - 1);
                mcells.Borders.Bottom.LineWeight = LineWeight.LineWeight025;
                mcells.Borders.Top.LineWeight = LineWeight.LineWeight025;
                mcells.Borders.Left.LineWeight = LineWeight.LineWeight025;
                mcells.Borders.Right.LineWeight = LineWeight.LineWeight025;

                rowIterator++;

            }
            
            rowIterator = headerRows;
            foreach (var type in attributes.Select(i => i.type).Distinct())
            {

                int rowCount = attributes.Where(i => i.type == type).ToList().Count() + 2;

                mcells = CellRange.Create(tbl, rowIterator, 0, rowIterator, columns - 1);
                mcells.BackgroundColor = blue;
                tbl.MergeCells(mcells);

                tbl.Cells[rowIterator, 0].SetValue(type, ParseOption.SetDefaultFormat);
                tbl.Cells[rowIterator, 0].Alignment = CellAlignment.MiddleLeft;
                tbl.Cells[rowIterator, 0].TextHeight = textHeight;

                rowIterator += rowCount;

            }

            mcells = CellRange.Create(tbl, bottomRow - 1, 0, bottomRow, columns - 1);
            mcells.Borders.Bottom.LineWeight = LineWeight.LineWeight025;
            mcells.Borders.Top.LineWeight = LineWeight.LineWeight025;
            mcells.Borders.Left.LineWeight = LineWeight.LineWeight025;
            mcells.Borders.Right.LineWeight = LineWeight.LineWeight025;

            tbl.Cells[rowIterator, 0].SetValue("TOTALS", ParseOption.SetDefaultFormat);
            tbl.Cells[rowIterator, 0].Alignment = CellAlignment.MiddleRight;
            tbl.Cells[rowIterator, 0].TextHeight = textHeight;

            cellIterator = 1;
            foreach (var type in attributes.Select(i => i.type).Distinct())
            {
                tbl.Cells[rowIterator, cellIterator].SetValue(attributes.Where(i => i.type == type).Sum(i => i.area).ToString(areaToString) + " " + append, ParseOption.SetDefaultFormat);
                tbl.Cells[rowIterator, cellIterator].Alignment = CellAlignment.MiddleCenter;
                tbl.Cells[rowIterator, cellIterator].TextHeight = textHeight;
                cellIterator++;
            }
            
            mcells = CellRange.Create(tbl, bottomRow, 0, bottomRow, columns - 1);
            mcells.BackgroundColor = red;
            tbl.MergeCells(mcells);

            tbl.Cells[bottomRow, 0].SetValue(attributes.Sum(i => i.area).ToString(areaToString) + " " + append, ParseOption.SetDefaultFormat);
            tbl.Cells[bottomRow, 0].Alignment = CellAlignment.MiddleCenter;
            tbl.Cells[bottomRow, 0].TextHeight = textHeight;

            tbl.GenerateLayout();

            ms.UpgradeOpen();
            ms.AppendEntity(tbl);
            tr.AddNewlyCreatedDBObject(tbl, true);

        }

        static public string FindObjectId(string text, out ObjectId objId)
        {

            const string prefix = "%<\\_ObjId ";
            const string suffix = ">%";
            
            // Find the location of the prefix string
            int preLoc = text.IndexOf(prefix);
            if (preLoc > 0)
            {
                // Find the location of the ID itself
                int idLoc = preLoc + prefix.Length;
                
                // Get the remaining string
                string remains = text.Substring(idLoc);
                
                // Find the location of the suffix
                int sufLoc = remains.IndexOf(suffix);
                
                // Extract the ID string and get the ObjectId
                string id = remains.Remove(sufLoc);
                IntPtr xAsIntPtr = new IntPtr(Convert.ToInt64(id));
                objId = new ObjectId(xAsIntPtr);
                
                // Return the remainder, to allow extraction of any remaining IDs
                return remains.Substring(sufLoc + suffix.Length);

            }
            else
            {
                objId = ObjectId.Null;
                return "";
            }

        }

        private static int GetIntFromString(string str)
        {
            return Convert.ToInt32(Regex.Replace(str, @"[^0-9]", ""));
        }

        [CommandMethod("EDC-BlockAddDiscipline")]
        public void BlockAddDiscipline()
        {

            Active.Database.UsingTransaction((tr) =>
            {

                try
                {

                    //Prompt user to select a polyline
                    PromptEntityOptions peo = new PromptEntityOptions("\nSelect a block: ");
                    peo.SetRejectMessage("\nNot a block try again: ");
                    peo.AddAllowedClass(typeof(BlockReference), true);

                    //Get the result
                    PromptEntityResult per;
                    per = Active.Editor.GetEntity(peo);

                    if (disciplineViewModels.Count == 0)
                        disciplineViewModels = DisciplineController._Get();

                    string discipline = AddPromptKeywordOptions(Active.Document, "Select a discipline: ", disciplineViewModels.Select(i => i.name).ToArray(), disciplineViewModels.Select(i => i.name).FirstOrDefault());
                    
                    //Find the block
                    BlockReference br = (BlockReference)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForWrite);
                    //tr.AddNewlyCreatedDBObject(btr, true);

                    //Add entities to block ---------------------------------------------------------------------
                    AttributeDefinition acAttDef = new AttributeDefinition();
                    acAttDef.Justify = AttachmentPoint.MiddleCenter;
                    acAttDef.AlignmentPoint = new Point3d((double)(-48 * GetConversionRate()), (double)(-48 * GetConversionRate()), 0);
                    acAttDef.Verifiable = true;
                    acAttDef.Prompt = "DISCIPLINE: ";
                    acAttDef.Tag = "DISCIPLINE";
                    acAttDef.TextString = discipline;
                    acAttDef.IsMTextAttributeDefinition = false;
                    acAttDef.Layer = "0";
                    acAttDef.Invisible = true;
                    acAttDef.Height = (double)(12 * GetConversionRate());
                    btr.AppendEntity(acAttDef);
                    tr.AddNewlyCreatedDBObject(acAttDef, true);
                    //Add entities to block ---------------------------------------------------------------------

                    // Report what we've done
                    Active.Editor.WriteMessage("\nAdded discipline attribute definition to {0}", btr.Name);

                }
                catch (Exception ex)
                {
                    // An exception has been thrown, indicating the name is invalid
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }

            });

        }

        public class BlockAddManyViewModel
        {

            public string discipline { get; set; }

            public string name { get; set; }
            
            public BlockAddManyViewModel()
            {
                discipline = "";
                name = "";
            }

        }

        [CommandMethod("EDC-BlockUpload")]
        public void BlockUpload()
        {

            Active.Database.UsingModelSpace((tr, ms) =>
            {

                try
                {

                    string m = (Active.Database.Measurement.ToString() == "English") ? "Imperial" : Active.Database.Measurement.ToString();
                    BlockTable bt = (BlockTable)tr.GetObject(Active.Database.BlockTableId, OpenMode.ForRead);
                    IEnumerable<BlockReference> brs = ms.OfType<BlockReference>(tr).Where(c => c.Name == "EDC-" + m.Substring(0, 1) + "-G-Tag Element");
                    List<BlockAddManyViewModel> bs = new List<BlockAddManyViewModel>();

                    PromptKeywordOptions pko = new PromptKeywordOptions("");
                    pko.Message = "\nUpload Blocks?";
                    pko.Keywords.Add("Yes");
                    pko.Keywords.Add("No");
                    PromptResult pr = Active.Editor.GetKeywords(pko);

                    if (pr.StringResult == "No")
                        throw new Exception("\nCanceled");

                    PromptStringOptions pso = new PromptStringOptions("\nEnter Password");
                    pr = Active.Editor.GetString(pso);

                    if (pr.StringResult != "5455")
                        throw new Exception("\nInvalid Password");

                    foreach (BlockReference br in brs)
                    {

                        //BlockTableRecord btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForWrite);
                        //if (btr.IsDependent || btr.Name != "EDC-" + m.Substring(0, 1) + "-G-Tag Element")
                        //    continue;
                        //The error occurs on the line below no clue why
                        //byte[] pi = layer.Transparency.Alpha;

                        //Active.Editor.WriteMessage("\n" + btr.Name);
                        //Active.Editor.WriteMessage("\nEDC-" + m.Substring(0, 1) + "-G-Tag Element");
                        ////Doesnt work
                        ////BlockReference blockRef = (BlockReference)tr.GetObject(block.ObjectId, OpenMode.ForWrite);

                        //// Verify block table record has attribute definitions associated with it
                        ////if (br.HasAttributeDefinitions)
                        ////{

                        //    string discipline = "";
                        //    string name = "";

                        //    // Add attributes from the block table record
                        //    foreach (ObjectId objID in btr)
                        //    {
                        //        DBObject dbObj = tr.GetObject(objID, OpenMode.ForWrite) as DBObject;

                        //        if (dbObj is AttributeDefinition)
                        //        {
                        //            AttributeDefinition ad = dbObj as AttributeDefinition;

                        //            Active.Editor.WriteMessage(string.Format("\nAttribute Definition {0}", ad.Tag));
                        //            Active.Editor.WriteMessage(string.Format("\nText String {0}", ad.TextString));
                        //            //Active.Editor.WriteMessage(string.Format("\nM Text {0}", ad.MTextAttributeDefinition));

                        //            //if (!ad.Constant)
                        //            //{
                        //            //    using (AttributeReference ar = new AttributeReference())
                        //            //    {
                        //            //        //ar.SetAttributeFromBlock(ad, ar.BlockTransform);
                        //            //        //ar.Position = ad.Position.TransformBy(ar.BlockTransform);

                        //            //        //ar.TextString = ad.TextString;

                        //            //        //btr.AttributeCollection.AppendAttribute(ar);
                        //            //        //tr.AddNewlyCreatedDBObject(ar, true);
                        //            //    }
                        //            //}
                        //            if (ad.Tag == "ELEMENT")
                        //                name = ad.TextString;
                        //            else if (ad.Tag == "DISCIPLINE")
                        //                discipline = ad.TextString;

                        //        }

                        //    }


                        string discipline = "";
                        string name = "";
                        foreach (ObjectId id in br.AttributeCollection)
                        {

                            DBObject dbObj = tr.GetObject(id, OpenMode.ForWrite) as DBObject;
                            AttributeReference aR = dbObj as AttributeReference;

                            if (aR.Tag == "ELEMENT")
                                name = aR.TextString;
                            else if (aR.Tag == "DISCIPLINE")
                                discipline = aR.TextString;

                        }

                        BlockAddManyViewModel b = new BlockAddManyViewModel()
                        {
                            discipline = discipline,
                            name = name
                        };

                        bs.Add(b);

                        //}

                    }

                    Active.Editor.WriteMessage("\nFound {0} Block(s)", bs.Count);
                    //LayerController._AddMany(ls);
                    foreach (var b in bs)
                        Active.Editor.WriteMessage("\nDiscipline: {0} / Name: {1}", b.discipline, b.name);

                }
                catch (Exception ex)
                {
                    Active.Editor.WriteMessage(String.Format("\nError: {0}", ex.Message));
                }

            });

        }

        //[CommandMethod("GetBlockIDs")]
        //public static void GetBlockIDs()
        //{

        //    var doc = Application.DocumentManager.MdiActiveDocument;
        //    var ed = doc.Editor;
        //    var db = doc.Database;

        //    ObjectIdCollection retCollection = new ObjectIdCollection();

        //    using (Transaction myTrans = db.TransactionManager.StartTransaction())
        //    {

        //        BlockTable myBT = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);

        //        if (myBT.Has("test3"))
        //        {

        //            BlockTableRecord myBTR = (BlockTableRecord)myBT["test3"].GetObject(OpenMode.ForRead);
        //            retCollection = (ObjectIdCollection)myBTR.GetBlockReferenceIds(true, true);
        //            ed.WriteMessage(retCollection.Count.ToString());

        //            myTrans.Commit();

        //            //return (retCollection);
        //        }
        //        else
        //        {
        //            myTrans.Commit();
        //            //return (retCollection);
        //        }

        //    }
        //}

        //[CommandMethod("AddBlockTest")]
        //static public void AddBlockTest()
        //{

        //    Database db = Application.DocumentManager.MdiActiveDocument.Database;

        //    using (Transaction myT = db.TransactionManager.StartTransaction())
        //    {

        //        //Get the block definition "Check".
        //        string blockName = "TagArea";
        //        BlockTable bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
        //        BlockTableRecord blockDef = bt[blockName].GetObject(OpenMode.ForRead) as BlockTableRecord;
        //        //Also open modelspace - we'll be adding our BlockReference to it
        //        BlockTableRecord ms = bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite) as BlockTableRecord;

        //        //Create new BlockReference, and link it to our block definition
        //        Point3d point = new Point3d(2.0, 4.0, 6.0);

        //        using (BlockReference blockRef = new BlockReference(point, blockDef.ObjectId))
        //        {
        //            //Add the block reference to modelspace
        //            ms.AppendEntity(blockRef);
        //            myT.AddNewlyCreatedDBObject(blockRef, true);
        //            //Iterate block definition to find all non-constant

        //            // AttributeDefinitions
        //            foreach (ObjectId id in blockDef)
        //            {

        //                DBObject obj = id.GetObject(OpenMode.ForRead);
        //                AttributeDefinition attDef = obj as AttributeDefinition;

        //                if ((attDef != null) && (!attDef.Constant))
        //                {
        //                    //This is a non-constant AttributeDefinition
        //                    //Create a new AttributeReference
        //                    using (AttributeReference attRef = new AttributeReference())
        //                    {
        //                        attRef.SetAttributeFromBlock(attDef, blockRef.BlockTransform);
        //                        attRef.TextString = "Hello World";
        //                        //Add the AttributeReference to the BlockReference
        //                        blockRef.AttributeCollection.AppendAttribute(attRef);
        //                        myT.AddNewlyCreatedDBObject(attRef, true);
        //                    }
        //                }

        //            }

        //        }
        //        //Our work here is done
        //        myT.Commit();
        //    }

        //}

        //Autodesk.AutoCAD.Runtime.CommandMethodAttribute
        //[CommandMethod("ChangeCircleColor", CommandFlags.UsePickSet)]
        //public static void ChangeCircleColor()
        //{

        //    Active.Database.UsingModelSpace((tr, ms) =>
        //    {

        //        var circles = ms.OfType<Circle>(tr).Where(c => c.Radius < 1.0).UpgradeOpen();

        //        //Loop through the entities in model space
        //        foreach (ObjectId objectId in circleIds)
        //        {

        //            var circle = (Circle)tr.GetObject(objectId, OpenMode.ForRead);

        //            if (circle.Radius < 1.0)
        //            {
        //                circle.UpgradeOpen();
        //                circle.ColorIndex = 1;
        //            }

        //        }

        //    });

        //}

    }
}

