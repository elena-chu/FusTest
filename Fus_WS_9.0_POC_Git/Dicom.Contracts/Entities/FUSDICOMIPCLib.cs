﻿
///////////////////////////////////////////////////////////////////////////////
/// DO NOT EDIT - autogenerated content
///////////////////////////////////////////////////////////////////////////////
namespace Ws.Dicom.Interfaces.Entities
{
    public enum FDCExamType
    {
        eFDCMRPLAN,
        eFDCMRPRIOR,
        eFDCCT

    }
    public enum FDCSeriesOrientation
    {
        eFDC_NO_ORIENTATION,
        eFDC_CORONAL,
        eFDC_AXIAL,
        eFDC_SAGITTAL,
        eFDC_OBLIQUE_CORONAL,
        eFDC_OBLIQUE_AXIAL,
        eFDC_OBLIQUE_SAGITTAL

    }
    public enum FDCSeriesAction
    {
        eFDC_NO_OPERATION,
        eFDC_LOAD,
        eFDC_REMOVE

    }
    public enum eFDCDCMClientMode
    {
        eFDC_NOMODE = 0,
        eFDC_BURN,
        eFDC_SESSION,
        eFDC_TREAT_PLANNING

    }
}