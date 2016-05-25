!
!               INTEL CORPORATION PROPRIETARY INFORMATION
!  This software is supplied under the terms of a license agreement or
!  nondisclosure agreement with Intel Corporation and may not be copied
!  or disclosed except in accordance with the terms of that agreement.
!    Copyright (c) 2003 Intel Corporation. All Rights Reserved.
!
! File    : mkl_dfti.f90
! Purpose : Intel(R) Math Kernel Library (MKL) interface for DFTI routines

module  MKL_DFT_TYPE

TYPE, PUBLIC :: DFTI_DESCRIPTOR
    PRIVATE
    CHARACTER(3) :: DFT_name      ! key "DFT"
    ! structure of this type is not used in Fortran code
    ! the pointer to this type is used only
END TYPE DFTI_DESCRIPTOR

! DFTI_CONFIG_PARAM for DftiSetValue and DftiGetValue funtions
 INTEGER, PARAMETER :: DFTI_FORWARD_DOMAIN = 0  ! Domain for forward transform, no default
 INTEGER, PARAMETER :: DFTI_DIMENSION      = 1  ! Dimension, no default
 INTEGER, PARAMETER :: DFTI_LENGTHS        = 2  ! length(s) of transform, no default
 INTEGER, PARAMETER :: DFTI_PRECISION      = 3  ! Precision of computation, no default
 INTEGER, PARAMETER :: DFTI_FORWARD_SCALE  = 4  ! Scale factor for forward transform, default = 1.0
 INTEGER, PARAMETER :: DFTI_BACKWARD_SCALE = 5  ! Scale factor for backward transform, default = 1.0
 INTEGER, PARAMETER :: DFTI_FORWARD_SIGN   = 6  ! Default for forward transform = DFTI_NEGATIVE
 INTEGER, PARAMETER :: DFTI_NUMBER_OF_TRANSFORMS = 7 ! Number of data sets to be transformed, default = 1
 INTEGER, PARAMETER :: DFTI_COMPLEX_STORAGE = 8      ! Representation for complex domain, default = DFTI_COMPLEX_COMPLEX
 INTEGER, PARAMETER :: DFTI_REAL_STORAGE    = 9      ! Rep. for real domain, default = DFTI_REAL_REAL
 INTEGER, PARAMETER :: DFTI_CONJUGATE_EVEN_STORAGE = 10 ! Rep. for conjugate even domain, default = DFTI_COMPLEX_REAL
 INTEGER, PARAMETER :: DFTI_PLACEMENT      = 11    ! Placement of result, default = DFTI_INPLACE
 INTEGER, PARAMETER :: DFTI_INPUT_STRIDES  = 12    ! Stride information of input data, default = tigthly
 INTEGER, PARAMETER :: DFTI_OUTPUT_STRIDES = 13    ! Stride information of output data, default = tigthly
 INTEGER, PARAMETER :: DFTI_INPUT_DISTANCE  = 14   ! Stride information of input data, default = 0
 INTEGER, PARAMETER :: DFTI_OUTPUT_DISTANCE = 15   ! Stride information of output data, default = 0
 INTEGER, PARAMETER :: DFTI_INITIALIZATION_EFFORT = 16 ! Effort spent in initialization, default = DFTI_MEDIUM
 INTEGER, PARAMETER :: DFTI_WORKSPACE   = 17    ! Use of workspace during computation, default = DFTI_ALLOW
 INTEGER, PARAMETER :: DFTI_ORDERING    = 18    ! Possible out of order computation, default = DFTI_ORDERED
 INTEGER, PARAMETER :: DFTI_TRANSPOSE   = 19    ! Possible transposition of result, default = DFTI_NONE
 INTEGER, PARAMETER :: DFTI_DESCRIPTOR_NAME = 20 ! name of descriptor, default = string of zero length
 INTEGER, PARAMETER :: DFTI_PACKED_FORMAT = 21 ! name of descriptor, default = string of zero length
! below for get_value functions only
 INTEGER, PARAMETER :: DFTI_COMMIT_STATUS = 22  ! Whether descriptor has been commited
 INTEGER, PARAMETER :: DFTI_VERSION       = 23  ! DFTI implementation version number
 INTEGER, PARAMETER :: DFTI_FORWARD_ORDERING = 24   ! the ordering of forward transform
 INTEGER, PARAMETER :: DFTI_BACKWARD_ORDERING = 25  ! the ordering of backward transform

!DFTI_CONFIG_VALUE
 INTEGER, PARAMETER :: DFTI_COMMITTED    = 30    ! status - commit
 INTEGER, PARAMETER :: DFTI_UNCOMMITTED  = 31    ! status - uncommit
 INTEGER, PARAMETER :: DFTI_COMPLEX      = 32    ! General domain
 INTEGER, PARAMETER :: DFTI_REAL         = 33    ! Real domain
 INTEGER, PARAMETER :: DFTI_CONJUGATE_EVEN = 34  ! Conjugate even domain
 INTEGER, PARAMETER :: DFTI_SINGLE       = 35    ! Single precision
 INTEGER, PARAMETER :: DFTI_DOUBLE       = 36    ! Double precision
 INTEGER, PARAMETER :: DFTI_NEGATIVE     = 37    ! -i, for setting definition of transform
 INTEGER, PARAMETER :: DFTI_POSITIVE     = 38    ! +i, for setting definition of transform
 INTEGER, PARAMETER :: DFTI_COMPLEX_COMPLEX = 39 ! Representation method for domain
 INTEGER, PARAMETER :: DFTI_COMPLEX_REAL   = 40 ! Representation method for domain
 INTEGER, PARAMETER :: DFTI_REAL_COMPLEX   = 41 ! Representation method for domain
 INTEGER, PARAMETER :: DFTI_REAL_REAL    = 42   ! Representation method for domain
 INTEGER, PARAMETER :: DFTI_INPLACE      = 43   ! Result overwrites input
 INTEGER, PARAMETER :: DFTI_NOT_INPLACE  = 44   ! Result placed differently than input
 INTEGER, PARAMETER :: DFTI_LOW          = 45   ! A low setting
 INTEGER, PARAMETER :: DFTI_MEDIUM       = 46   ! A medium setting
 INTEGER, PARAMETER :: DFTI_HIGH         = 47   ! A high setting
 INTEGER, PARAMETER :: DFTI_ORDERED      = 48   ! Data on forward and backward domain ordered
 INTEGER, PARAMETER :: DFTI_BACKWARD_SCRAMBLED = 49 ! Data on forward ordered and backward domain scrambled
 INTEGER, PARAMETER :: DFTI_FORWARD_SCRAMBLED  = 50 ! Data on forward scrambled and backward domain ordered
 INTEGER, PARAMETER :: DFTI_ALLOW        = 51   ! Allow certain request or usage
 INTEGER, PARAMETER :: DFTI_AVOID        = 52   ! Avoid certain request or usage
 INTEGER, PARAMETER :: DFTI_NONE         = 53   ! none certain request or usage
 INTEGER, PARAMETER :: DFTI_CCS_FORMAT   = 54   ! ccs format for real DFT
 INTEGER, PARAMETER :: DFTI_PACK_FORMAT  = 55   ! pack format for real DFT
 INTEGER, PARAMETER :: DFTI_PERM_FORMAT  = 56   ! perm format for real DFT

!  returned error code
 INTEGER, PARAMETER :: DFTI_NO_ERROR                   = 0
 INTEGER, PARAMETER :: DFTI_MEMORY_ERROR               = 1
 INTEGER, PARAMETER :: DFTI_INVALID_CONFIGURATION      = 2
 INTEGER, PARAMETER :: DFTI_INCONSISTENT_CONFIGURATION = 3
 INTEGER, PARAMETER :: DFTI_MULTITHREADED_ERROR        = 4
 INTEGER, PARAMETER :: DFTI_BAD_DESCRIPTOR             = 5
 INTEGER, PARAMETER :: DFTI_UNIMPLEMENTED              = 6

 INTEGER, PARAMETER :: DFTI_MAX_MESSAGE_LENGTH = 40   !  maximum LENGTH of the error string
 INTEGER, PARAMETER :: DFTI_MAX_NAME_LENGTH = 10      !  maximum LENGTH of the desc user name
 INTEGER, PARAMETER :: DFTI_VERSION_LENGTH = 100      !  maximum LENGTH of the MKL version
 INTEGER, PARAMETER :: DFTI_ERROR_CLASS = 60          !  number of error class

end module MKL_DFT_TYPE

module  MKL_DFTI    

    USE MKL_DFT_TYPE

    INTERFACE DftiCreateDescriptor

        FUNCTION dfti_create_descriptor_1d(DFTI_Desc, precision, domain, dim, length)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_create_descriptor_1d
            !MS$ATTRIBUTES REFERENCE :: precision
            !MS$ATTRIBUTES REFERENCE :: domain
            !MS$ATTRIBUTES REFERENCE :: dim
            !MS$ATTRIBUTES REFERENCE :: length
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_create_descriptor_1d
            INTEGER(4), INTENT(IN) :: precision
            INTEGER(4), INTENT(IN) :: domain
            INTEGER(4), INTENT(IN) :: dim, length
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_create_descriptor_1d

        FUNCTION dfti_create_descriptor_highd(DFTI_Desc, precision, domain, dim, length)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_create_descriptor_highd
            !MS$ATTRIBUTES REFERENCE :: precision
            !MS$ATTRIBUTES REFERENCE :: domain
            !MS$ATTRIBUTES REFERENCE :: dim
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_create_descriptor_highd
            INTEGER(4), INTENT(IN) :: precision
            INTEGER(4), INTENT(IN) :: domain
            INTEGER(4), INTENT(IN) :: dim
            INTEGER(4), INTENT(IN), DIMENSION(*) :: length
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_create_descriptor_highd
    
    END INTERFACE

    INTERFACE DftiCommitDescriptor

        FUNCTION dfti_commit_descriptor_external(DFTI_Desc)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_commit_descriptor_external
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_commit_descriptor_external
            TYPE(DFTI_DESCRIPTOR), POINTER ::DFTI_Desc
        END FUNCTION dfti_commit_descriptor_external
    END INTERFACE

    INTERFACE DftiSetValue

        FUNCTION dfti_set_value_intval(DFTI_Desc, OptName, IntVal)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_set_value_intval
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: IntVal
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_set_value_intval
            INTEGER(4), INTENT(IN) :: OptName
            INTEGER(4), INTENT(IN) :: IntVal
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_set_value_intval

        FUNCTION dfti_set_value_sglval(DFTI_Desc, OptName, sglval)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_set_value_sglval
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: sglval
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_set_value_sglval
            INTEGER(4), INTENT(IN) :: OptName
            REAL(4), INTENT(IN) :: sglval
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_set_value_sglval

        FUNCTION dfti_set_value_dblval(DFTI_Desc, OptName, DblVal)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_set_value_dblval
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: DblVal
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_set_value_dblval
            INTEGER(4), INTENT(IN) :: OptName
            REAL(8), INTENT(IN) :: DblVal
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_set_value_dblval

        FUNCTION dfti_set_value_intvec(DFTI_Desc, OptName, IntVec)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_set_value_intvec
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: IntVec
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_set_value_intvec
            INTEGER(4), INTENT(IN) :: OptName
            INTEGER(4), INTENT(IN), DIMENSION(*) :: IntVec
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_set_value_intvec

        FUNCTION dfti_set_value_chars(DFTI_Desc, OptName, Chars)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_set_value_chars
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: Chars
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_set_value_chars
            INTEGER(4), INTENT(IN) :: OptName
            CHARACTER(*), INTENT(IN) :: Chars
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_set_value_chars

    END INTERFACE

    INTERFACE DftiGetValue

        FUNCTION dfti_get_value_intval(DFTI_Desc, OptName, IntVal)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_get_value_intval
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: IntVal
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_get_value_intval
            INTEGER(4), INTENT(IN) :: OptName
            INTEGER(4), INTENT(OUT) :: IntVal
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_get_value_intval

        FUNCTION dfti_get_value_sglval(DFTI_Desc, OptName, sglval)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_get_value_sglval
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: sglval
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_get_value_sglval
            INTEGER(4), INTENT(IN) :: OptName
            REAL(4), INTENT(OUT) :: sglval
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_get_value_sglval

        FUNCTION dfti_get_value_dblval(DFTI_Desc, OptName, DblVal)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_get_value_dblval
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: DblVal
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_get_value_dblval
            INTEGER(4), INTENT(IN) :: OptName
            REAL(8), INTENT(OUT) :: DblVal
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_get_value_dblval

        FUNCTION dfti_get_value_intvec(DFTI_Desc, OptName, IntVec)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_get_value_intvec
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: IntVec
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_get_value_intvec
            INTEGER(4), INTENT(IN) :: OptName
            INTEGER(4), INTENT(OUT), DIMENSION(*) :: IntVec
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_get_value_intvec

        FUNCTION dfti_get_value_chars(DFTI_Desc, OptName, Chars)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_get_value_chars
            !MS$ATTRIBUTES REFERENCE :: OptName
            !MS$ATTRIBUTES REFERENCE :: Chars
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_get_value_chars
            INTEGER(4), INTENT(IN) :: OptName
            CHARACTER(*), INTENT(OUT) :: Chars
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_get_value_chars

    END INTERFACE

    INTERFACE DftiComputeForward

        FUNCTION dfti_compute_forward_c(DFTI_Desc, a_tst)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_forward_c
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_forward_c
            COMPLEX(4), INTENT(INOUT), DIMENSION(*) :: a_tst
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_forward_c

        FUNCTION dfti_compute_forward_z(DFTI_Desc, a_tst)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_forward_z
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_forward_z
            COMPLEX(8), INTENT(INOUT), DIMENSION(*) :: a_tst
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_forward_z

        FUNCTION dfti_compute_forward_c_out(DFTI_Desc, a_tst, a_tst_out)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_forward_c_out
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_forward_c_out
            COMPLEX(4), INTENT(IN), DIMENSION(*) :: a_tst
            COMPLEX(4), INTENT(OUT), DIMENSION(*) :: a_tst_out
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_forward_c_out

        FUNCTION dfti_compute_forward_z_out(DFTI_Desc, a_tst, a_tst_out)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_forward_z_out
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_forward_z_out
            COMPLEX(8), INTENT(IN), DIMENSION(*) :: a_tst
            COMPLEX(8), INTENT(OUT), DIMENSION(*) :: a_tst_out
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_forward_z_out

        FUNCTION dfti_compute_forward_s(DFTI_Desc, a_tst)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_forward_s
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_forward_s
            REAL(4), INTENT(INOUT), DIMENSION(*) :: a_tst
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_forward_s

        FUNCTION dfti_compute_forward_d(DFTI_Desc, a_tst)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_forward_d
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_forward_d
            REAL(8), INTENT(INOUT), DIMENSION(*) :: a_tst
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_forward_d

        FUNCTION dfti_compute_forward_s_out(DFTI_Desc, a_tst, a_tst_out)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_forward_s_out
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_forward_s_out
            REAL(4), INTENT(IN), DIMENSION(*) :: a_tst
            REAL(4), INTENT(OUT), DIMENSION(*) :: a_tst_out
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_forward_s_out

        FUNCTION dfti_compute_forward_d_out(DFTI_Desc, a_tst, a_tst_out)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_forward_d_out
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_forward_d_out
            REAL(8), INTENT(IN), DIMENSION(*) :: a_tst
            REAL(8), INTENT(OUT), DIMENSION(*) :: a_tst_out
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_forward_d_out

    END INTERFACE

    INTERFACE DftiComputeBackward

        FUNCTION dfti_compute_backward_c(DFTI_Desc , a_tst)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_backward_c
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_backward_c
            COMPLEX(4), INTENT(INOUT), DIMENSION(*) :: a_tst
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_backward_c

        FUNCTION dfti_compute_backward_z(DFTI_Desc , a_tst)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_backward_z
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_backward_z
            COMPLEX(8), INTENT(INOUT), DIMENSION(*) :: a_tst
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_backward_z

        FUNCTION dfti_compute_backward_c_out(DFTI_Desc , a_tst, a_tst_out)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_backward_c_out
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_backward_c_out
            COMPLEX(4), INTENT(IN), DIMENSION(*) :: a_tst
            COMPLEX(4), INTENT(OUT), DIMENSION(*) :: a_tst_out
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_backward_c_out

        FUNCTION dfti_compute_backward_z_out(DFTI_Desc , a_tst, a_tst_out)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_backward_z_out
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_backward_z_out
            COMPLEX(8), INTENT(IN), DIMENSION(*) :: a_tst
            COMPLEX(8), INTENT(OUT), DIMENSION(*) :: a_tst_out
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_backward_z_out

        FUNCTION dfti_compute_backward_s(DFTI_Desc , a_tst)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_backward_s
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_backward_s
            REAL(4), INTENT(INOUT), DIMENSION(*) :: a_tst
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_backward_s

        FUNCTION dfti_compute_backward_d(DFTI_Desc , a_tst)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_backward_d
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_backward_d
            REAL(8), INTENT(INOUT), DIMENSION(*) :: a_tst
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_backward_d

        FUNCTION dfti_compute_backward_s_out(DFTI_Desc , a_tst, a_tst_out)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_backward_s_out
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_backward_s_out
            REAL(4), INTENT(IN), DIMENSION(*) :: a_tst
            REAL(4), INTENT(OUT), DIMENSION(*) :: a_tst_out
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_backward_s_out

        FUNCTION dfti_compute_backward_d_out(DFTI_Desc , a_tst, a_tst_out)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_compute_backward_d_out
            !MS$ATTRIBUTES REFERENCE :: DFTI_Desc
            INTEGER(4) dfti_compute_backward_d_out
            REAL(8), INTENT(IN), DIMENSION(*) :: a_tst
            REAL(8), INTENT(OUT), DIMENSION(*) :: a_tst_out
            TYPE(DFTI_DESCRIPTOR), POINTER :: DFTI_Desc
        END FUNCTION dfti_compute_backward_d_out

    END INTERFACE

    INTERFACE DftiFreeDescriptor
        FUNCTION dfti_free_descriptor_external(hand_c1d)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_free_descriptor_external
            !MS$ATTRIBUTES REFERENCE :: hand_c1d
            INTEGER(4) dfti_free_descriptor_external
            TYPE(DFTI_DESCRIPTOR), POINTER :: hand_c1d
        END FUNCTION dfti_free_descriptor_external
    END INTERFACE

    INTERFACE DftiErrorClass
        FUNCTION dfti_error_class_external(Status, ErrorClass)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_error_class_external
            !MS$ATTRIBUTES REFERENCE :: Status
            !MS$ATTRIBUTES REFERENCE :: ErrorClass
            LOGICAL dfti_error_class_external
            INTEGER(4), INTENT(IN) :: Status
            INTEGER(4), INTENT(IN) :: ErrorClass
        END FUNCTION dfti_error_class_external
    END INTERFACE

    INTERFACE DftiErrorMessage
        FUNCTION dfti_error_message_external(Status)
            USE MKL_DFT_TYPE
            !DEC$ATTRIBUTES C :: dfti_error_message_external
            !MS$ATTRIBUTES REFERENCE :: Status
            CHARACTER(LEN=DFTI_MAX_MESSAGE_LENGTH) ::  dfti_error_message_external
            INTEGER(4), INTENT(IN) :: Status
        END FUNCTION DFTI_ERROR_MESSAGE_EXTERNAL
    END INTERFACE

end module MKL_DFTI

