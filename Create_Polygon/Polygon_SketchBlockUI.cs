﻿//==============================================================================
//  WARNING!!  This file is overwritten by the Block UI Styler while generating
//  the automation code. Any modifications to this file will be lost after
//  generating the code again.
//
//       Filename: C:\Users\MOHAN DULAM\Documents\Visual Studio 2019\NX_Open\LN-Projects\Create_Polygon\Create_Polygon
//       //
//        This file was generated by the NX Block UI Styler
//        Created by: MOHAN DULAM
//              Version: NX 12
//              Date: 10-24-2023  (Format: mm-dd-yyyy)
//              Time: 16:16 (Format: hh-mm)
//
//==============================================================================

//==============================================================================
//  Purpose:  This TEMPLATE file contains C# source to guide you in the
//  construction of your Block application dialog. The generation of your
//  dialog file (.dlx extension) is the first step towards dialog construction
//  within NX.  You must now create a NX Open application that
//  utilizes this file (.dlx).
//
//  The information in this file provides you with the following:
//
//  1.  Help on how to load and display your Block UI Styler dialog in NX
//      using APIs provided in NXOpen.BlockStyler namespace
//  2.  The empty callback methods (stubs) associated with your dialog items
//      have also been placed in this file. These empty methods have been
//      created simply to start you along with your coding requirements.
//      The method name, argument list and possible return values have already
//      been provided for you.
//==============================================================================

//------------------------------------------------------------------------------
//These imports are needed for the following template code
//------------------------------------------------------------------------------
using System;
using NXOpen;
using NXOpen.BlockStyler;

//------------------------------------------------------------------------------
//Represents Block Styler application class
//------------------------------------------------------------------------------
public class Polygon_SketchBlockUI
{
    //class members
    private static Session theSession = null;
    private static UI theUI = null;
    private string theDlxFileName;
    private NXOpen.BlockStyler.BlockDialog theDialog;
    private NXOpen.BlockStyler.SpecifyPlane planeSketch;// Block type: Specify Plane
    private NXOpen.BlockStyler.IntegerBlock intNoOfSides;// Block type: Integer
    private NXOpen.BlockStyler.DoubleBlock dblSideLength;// Block type: Double
    private NXOpen.BlockStyler.Toggle tglFilletRadius;// Block type: Toggle
    private NXOpen.BlockStyler.DoubleBlock dblFilletRadius;// Block type: Double
    private NXOpen.BlockStyler.Toggle tglExtrude;// Block type: Toggle
    private NXOpen.BlockStyler.DoubleBlock dblExtrudeLength;// Block type: Double
    private NXOpen.BlockStyler.Group group0;// Block type: Group
    // my Variables
    private int numberofSidesOfPolygon;
    private double lengthOfSide;
    private double filletRadius;
    private double extrydeLength;
    private string dllFileLoc; // Declerartion DLL File Path

    //------------------------------------------------------------------------------
    //Constructor for NX Styler class
    //------------------------------------------------------------------------------
    public Polygon_SketchBlockUI()
    {
        try
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            // DLL File Path
            dllFileLoc = System.Reflection.Assembly.GetExecutingAssembly().Location;
            dllFileLoc = dllFileLoc.Substring(0, dllFileLoc.LastIndexOf("\\"));
            theDlxFileName = dllFileLoc + "\\Polygon_Sketch-BlockUI.dlx";
            theDialog = theUI.CreateDialog(theDlxFileName);
            theDialog.AddApplyHandler(new NXOpen.BlockStyler.BlockDialog.Apply(apply_cb));
            theDialog.AddOkHandler(new NXOpen.BlockStyler.BlockDialog.Ok(ok_cb));
            theDialog.AddUpdateHandler(new NXOpen.BlockStyler.BlockDialog.Update(update_cb));
            theDialog.AddInitializeHandler(new NXOpen.BlockStyler.BlockDialog.Initialize(initialize_cb));
            theDialog.AddDialogShownHandler(new NXOpen.BlockStyler.BlockDialog.DialogShown(dialogShown_cb));
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            throw ex;
        }
    }
    //------------------------------- DIALOG LAUNCHING ---------------------------------
    //
    //    Before invoking this application one needs to open any part/empty part in NX
    //    because of the behavior of the blocks.
    //
    //    Make sure the dlx file is in one of the following locations:
    //        1.) From where NX session is launched
    //        2.) $UGII_USER_DIR/application
    //        3.) For released applications, using UGII_CUSTOM_DIRECTORY_FILE is highly
    //            recommended. This variable is set to a full directory path to a file 
    //            containing a list of root directories for all custom applications.
    //            e.g., UGII_CUSTOM_DIRECTORY_FILE=$UGII_BASE_DIR\ugii\menus\custom_dirs.dat
    //
    //    You can create the dialog using one of the following way:
    //
    //    1. Journal Replay
    //
    //        1) Replay this file through Tool->Journal->Play Menu.
    //
    //    2. USER EXIT
    //
    //        1) Create the Shared Library -- Refer "Block UI Styler programmer's guide"
    //        2) Invoke the Shared Library through File->Execute->NX Open menu.
    //
    //------------------------------------------------------------------------------
    public static void PolygonSketchMain()
    {
        Polygon_SketchBlockUI thePolygon_SketchBlockUI = null;
        try
        {
            thePolygon_SketchBlockUI = new Polygon_SketchBlockUI();
            // The following method shows the dialog immediately
            thePolygon_SketchBlockUI.Show();
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        finally
        {
            if(thePolygon_SketchBlockUI != null)
                thePolygon_SketchBlockUI.Dispose();
                thePolygon_SketchBlockUI = null;
        }
    }
    //------------------------------------------------------------------------------
    // This method specifies how a shared image is unloaded from memory
    // within NX. This method gives you the capability to unload an
    // internal NX Open application or user  exit from NX. Specify any
    // one of the three constants as a return value to determine the type
    // of unload to perform:
    //
    //
    //    Immediately : unload the library as soon as the automation program has completed
    //    Explicitly  : unload the library from the "Unload Shared Image" dialog
    //    AtTermination : unload the library when the NX session terminates
    //
    //
    // NOTE:  A program which associates NX Open applications with the menubar
    // MUST NOT use this option since it will UNLOAD your NX Open application image
    // from the menubar.
    //------------------------------------------------------------------------------
     public static int GetUnloadOption(string arg)
    {
        //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
         return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
        // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }
    
    //------------------------------------------------------------------------------
    // Following method cleanup any housekeeping chores that may be needed.
    // This method is automatically called by NX.
    //------------------------------------------------------------------------------
    public static void UnloadLibrary(string arg)
    {
        try
        {
            //---- Enter your code here -----
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }
    
    //------------------------------------------------------------------------------
    //This method shows the dialog on the screen
    //------------------------------------------------------------------------------
    public NXOpen.UIStyler.DialogResponse Show()
    {
        try
        {
            theDialog.Show();
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return 0;
    }
    
    //------------------------------------------------------------------------------
    //Method Name: Dispose
    //------------------------------------------------------------------------------
    public void Dispose()
    {
        if(theDialog != null)
        {
            theDialog.Dispose();
            theDialog = null;
        }
    }
    
    //------------------------------------------------------------------------------
    //---------------------Block UI Styler Callback Functions--------------------------
    //------------------------------------------------------------------------------
    
    //------------------------------------------------------------------------------
    //Callback Name: initialize_cb
    //------------------------------------------------------------------------------
    public void initialize_cb()
    {
        try
        {
            planeSketch = (NXOpen.BlockStyler.SpecifyPlane)theDialog.TopBlock.FindBlock("planeSketch");
            intNoOfSides = (NXOpen.BlockStyler.IntegerBlock)theDialog.TopBlock.FindBlock("intNoOfSides");
            dblSideLength = (NXOpen.BlockStyler.DoubleBlock)theDialog.TopBlock.FindBlock("dblSideLength");
            tglFilletRadius = (NXOpen.BlockStyler.Toggle)theDialog.TopBlock.FindBlock("tglFilletRadius");
            dblFilletRadius = (NXOpen.BlockStyler.DoubleBlock)theDialog.TopBlock.FindBlock("dblFilletRadius");
            tglExtrude = (NXOpen.BlockStyler.Toggle)theDialog.TopBlock.FindBlock("tglExtrude");
            dblExtrudeLength = (NXOpen.BlockStyler.DoubleBlock)theDialog.TopBlock.FindBlock("dblExtrudeLength");
            group0 = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group0");
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }
    
    //------------------------------------------------------------------------------
    //Callback Name: dialogShown_cb
    //This callback is executed just before the dialog launch. Thus any value set 
    //here will take precedence and dialog will be launched showing that value. 
    //------------------------------------------------------------------------------
    public void dialogShown_cb()
    {
        try
        {
            //---- Enter your callback code here -----
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }
    
    //------------------------------------------------------------------------------
    //Callback Name: apply_cb
    //------------------------------------------------------------------------------
    public int apply_cb()
    {
        int errorCode = 0;
        try
        {
            //---- Enter your callback code here -----
            // Parameters from Block UI
            TaggedObject[] selecteddatumPlane = planeSketch.GetSelectedObjects();
            PropertyList numberofSidesProp = intNoOfSides.GetProperties();
            PropertyList sideLengthProp = dblSideLength.GetProperties();
            PropertyList filletRadiusProp = dblFilletRadius.GetProperties();
            PropertyList extrudeLengthProp = dblExtrudeLength.GetProperties();

            // Check to select one Datum Plane
            if (selecteddatumPlane.Length < 1) 
            {
                theUI.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, "Select only one Plane");
            }
            else
            {
                // Convert selected Datum Palne to Plane
                Plane datumPlane = (Plane)selecteddatumPlane[0];

                // Check mininum sides for polygon is 3
                if (numberofSidesProp.GetInteger("Value") >= 3) 
                {
                    // Assign value to numberofSidesOfPolygon
                    numberofSidesOfPolygon = numberofSidesProp.GetInteger("Value"); 
                    numberofSidesProp.Dispose();
                }
                else
                {
                    theUI.NXMessageBox.Show("Error", NXMessageBox.DialogType.Information,
                        "Side of the Polygon should be Greater than or equal to 3");
                }

                // Assign value to lengthOfSide
                lengthOfSide = sideLengthProp.GetDouble("Value"); 
                sideLengthProp.Dispose();

                // Fillet radius must be less than side length of Polygon
                if (filletRadiusProp.GetDouble("Value") < lengthOfSide) 
                {
                    // Check toggle is Enabled
                    if (tglFilletRadius.Value==true) 
                        filletRadius = filletRadiusProp.GetDouble("Value"); // Assign value to filletRadius

                    filletRadiusProp.Dispose();
                }
                else
                {
                    theUI.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, 
                        "Provide proper fillet radius value");
                }

                // Check Extrude length should not be zero and toggle is Enabled
                if (extrudeLengthProp.GetDouble("Value") > 0 && tglExtrude.Value == true) 
                {
                    // Assign value to extrude Length
                    extrydeLength = extrudeLengthProp.GetDouble("Value"); 
                    extrudeLengthProp.Dispose();
                }

                // Check mininum sides for polygon is 3
                if (numberofSidesOfPolygon >= 3) 
                {
                    PolygonSketch.Polygon.CreatePolygonSketch(datumPlane, numberofSidesOfPolygon, lengthOfSide, filletRadius, extrydeLength);
                    
                }
                else
                {
                    theUI.NXMessageBox.Show("Information", NXMessageBox.DialogType.Information, "Polygon is not possible with given No of Sides");
                }
            }
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            errorCode = 1;
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return errorCode;
    }
    
    //------------------------------------------------------------------------------
    //Callback Name: update_cb
    //------------------------------------------------------------------------------
    public int update_cb( NXOpen.BlockStyler.UIBlock block)
    {
        try
        {
            if(block == planeSketch)
            {
            //---------Enter your code here-----------
            }
            else if(block == intNoOfSides)
            {
            //---------Enter your code here-----------
            }
            else if(block == dblSideLength)
            {
            //---------Enter your code here-----------
            }
            else if(block == tglFilletRadius)
            {
                //---------Enter your code here-----------
                // check toggle is Enabled
                if (tglFilletRadius.Value == true) 
                    dblFilletRadius.Enable = true; // enable the Fillet Radius value
                else
                    dblFilletRadius.Enable = false;
            }
            else if(block == dblFilletRadius)
            {
            //---------Enter your code here-----------
            }
            else if(block == tglExtrude)
            {
                //---------Enter your code here-----------
                // check toggle is Enabled
                if (tglExtrude.Value == true) 
                    dblExtrudeLength.Enable = true; // enable the Extrude Length value
                else
                    dblExtrudeLength.Enable = false;
            }
            else if(block == dblExtrudeLength)
            {
            //---------Enter your code here-----------
            }
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return 0;
    }
    
    //------------------------------------------------------------------------------
    //Callback Name: ok_cb
    //------------------------------------------------------------------------------
    public int ok_cb()
    {
        int errorCode = 0;
        try
        {
            errorCode = apply_cb();
            //---- Enter your callback code here -----
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            errorCode = 1;
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return errorCode;
    }
    //------------------------------------------------------------------------------
    //Function Name: GetBlockProperties
    //Returns the propertylist of the specified BlockID
    //------------------------------------------------------------------------------
    public PropertyList GetBlockProperties(string blockID)
    {
        PropertyList plist =null;
        try
        {
            plist = theDialog.GetBlockProperties(blockID);
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return plist;
    }
    
}
