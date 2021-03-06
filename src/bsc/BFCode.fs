// Copyright (c) 2016 Theodore Tsirpanis
// 
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
namespace Brainsharp

type BFToken = 
    | MemoryControl of byte
    | MemorySet of byte
    | PointerControl of int
    | IOWrite
    | IORead
    | Loop of BFToken list

module BFCode = 
    open BFParser
    open System
    
    let rec makeCodeTree s = 
        s |> List.map (function 
                 | Plus -> MemoryControl 1uy
                 | Minus -> MemoryControl 255uy
                 | Left -> PointerControl -1
                 | Right -> PointerControl 1
                 | Dot -> IOWrite
                 | Comma -> IORead
                 | BracketLoop body -> Loop(makeCodeTree body))
