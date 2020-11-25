﻿// =================================
// This file is part of a complete app example.
// Part 3. The DTOs that can be converted to/from Domain objects
//
// Exercise:
//    look at, execute, and understand all the code in this file
// =================================

open System


// uncomment when working with this file standalone
// comment when including this file in another file
(*
// Load a file with library functions for Result
#load "Result.fsx"

// Load the domain
#load "03a-CompleteApp-Domain.fsx"
open ``03a-CompleteApp-Domain``
*)

//===============================================
// Implementation of the DTOs 
//===============================================

module Dto =
    open EmailServiceDomain

    [<CLIMutableAttribute>]  // This is needed for JSON, so ignore this for now
    type RequestDto = {
        UserId: int
        FromAddress: string
        ToAddress: string
        Body: string
    }

    module RequestDto =

        /// Convert a dto into a Domain.Request,
        /// and do validation at the same time
        let toDomain (dto:RequestDto) :Validation<Request,ValidationError> =

            // a "constructor" function
            let createRequest userId fromAddress toAddress emailBody =
               {UserId=userId; FromAddress=fromAddress; ToAddress=toAddress; EmailBody=emailBody}

            // the validated components
            let userIdOrError =
                dto.UserId
                |> UserId.create         // a Result with a single error
                |> Validation.ofResult   // convert to list of errors
            let fromAddressOrError =
                dto.FromAddress
                |> EmailAddress.create "fromAddress"
                |> Validation.ofResult
            let toAddressOrError =
                dto.ToAddress
                |> EmailAddress.create "toAddress"
                |> Validation.ofResult
            let emailBodyOrError =
                dto.Body
                |> EmailBody.create      // a Result with a single error
                |> Validation.ofResult   // convert to list of errors

            // use the "lift4" function (because there are four parameters)
            let requestOrError =
                (Validation.lift4 createRequest) userIdOrError fromAddressOrError toAddressOrError emailBodyOrError

            requestOrError



//===========================================
// Some interactive tests
//===========================================

module Test = 
    open EmailServiceDomain
    open Dto

    let goodRequestDto : RequestDto = {
        UserId = 1
        FromAddress = "abc@gmail.com"
        ToAddress = "xyz@gmail.com"
        Body = "Hello"
        }

    let badRequestDto : RequestDto  = {
        UserId = 0
        FromAddress = "gmail.com"
        ToAddress = ""
        Body = ""
        }

    let invalidEmailDto : RequestDto  = {
        UserId = 2
        FromAddress = "abc@example.com"
        ToAddress = "xyz@example.com"
        Body = "Hello"
        }

    // TEST: try the good and bad DTOs
    goodRequestDto |> RequestDto.toDomain
    badRequestDto |> RequestDto.toDomain
    invalidEmailDto |> RequestDto.toDomain


