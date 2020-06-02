### File with all the VRAD related functions ###
export VRediting
#using Atom
using CodeTools
#using Cassette
using MacroTools
#States
#0.Deactivated
#1.Main
#2.InSelection
#3.Selected

#Variables
active = false
state = 0
hasChanges = false
fileAndLinesOfSelected = nothing
callChainDict = Dict()
selected = nothing
newFile = false
changeHistory = []
test = "D:\\Catarina\\OneDrive - Universidade de Lisboa\\IST\\Tese\\Code\\simple.jl"
test2 = "D:\\Catarina\\Transferências\\isenberg_BIM.jl"
source_file = ""
#Functions
@defop myfunction(prompt::String="nothing") #myfunction is a test function to start cycle
@defop queryUnity(prompt::String="nothing")
@defop startSelected(selected)
@defop VRediting(prompt::String="nothing")
@defop sendFamilyData()
@defop openAddPanel()
@defop sendGlobalVariables()

#=
lista = ["building", "column"]
Cassette.@context Inspect
Cassette.prehook(::Inspect, f, args...) = let
    if (string(f) in lista)
        println(Base.@locals)
    end
end
=#

struct Change
    type
    file
    line_number::Int
    size::Int
    old_code
    new_code
    #TODO: automatically calculate size
end

function applyChange(c::Change)
    push!(changeHistory, c)
    writeLine(c.file, c.line_number, c.new_code)
    runFile(c.file)
end

function removeChange()
    c = pop!(changeHistory)
    #writeLine
end

function VRediting(file::String, b::Unity)
    print("--> VRAD is active")
    global source_file = file
    active = true
    state = 1
    @remote(b, EnableVRAD())
end

function myfunction(prompt::String, b::Unity)
    try
    @async queryUnity()
    catch e
          showerror(stdout, e)
     end
end

function cassette()
    Cassette.overdub(Inspect(), CodeTools.getthing("building") , xy(0,0), 10, 8)
end


function queryUnity(prompt::String, b::Unity)
     while true
         sleep(0.1)
         let s = @remote(b, CallJulia())
         if (s != "empty")
             splitted = split(s, "#")
             println("--> Query Unity result: ", splitted[2:end])
             if (length(splitted) > 1)
                 runFunction(splitted[1], splitted[2:end])
             else
                 runFunction(s)
             end
         end
     end
   end
 end


runFunction(name) = getfield(Khepri, Symbol(name))()
runFunction(name, args) = getfield(Khepri, Symbol(name))(args)


function startInSelection()
     try
         state = 2
         println("--> Editing started")
         global selected = select_shape()
         print(selected,"\n")
         global fileAndLinesOfSelected = reverseHighlight(selected)
         startSelected(selected)
     catch e
         showerror(stdout, e)
     end
end

function startSelected(selected, b::Unity)
    state = 3
    println("--> Selected object: ", selected)
    try
        @remote(b, ChangeState(2))
    catch Exception
        println("ERROR: ChangeState")
    end
end

function stopEditingMode()
    #save state?
    state = 1
    println("--> Editing stopped")
end

# () -> [FamilyType, ParameterName, ParameterValue...]
# TODO: Add material
function sendFamilyData(b::Unity)
    try
        fields = fieldnames(typeof(Khepri.selected.family))[1:end-3]
        print(fields)
        data = ["$(typeof(Khepri.selected.family))"]
        foreach(x -> append!(data, ["$x", "$(getproperty(Khepri.selected.family, x))"]), fields)
        names = extractCallChainNames()
        foreach(x -> append!(data, [x]), names)
        println("--> Family data: ", data)
        @remote(b, StoreFamilyData(data))
    catch e
        showerror(stdout, e)
    end
end

function sendAvailableFamilies(family, b::Unity)
    try
        if (!isnothing(selected))
            data = []
            families = available_families(selected.family)
            for family in families
                res = ""
                foreach(x-> res = res * string(getfield(family,x)) * "#", fieldnames(typeof(family))[1:end-3])
                res = res * collect(values(family.implemented_as))[1].name
                append!(data, [res])
            end
            @remote(b, StoreAvailableFamiliesData(data))
        else
            data = []
            families = available_families(family)
            for family in families
                res = ""
                foreach(x-> res = res * string(getfield(family,x)) * "#", fieldnames(typeof(family))[1:end-3])
                res = res * collect(values(family.implemented_as))[1].name
                append!(data, [res])
            end
            @remote(b, StoreAvailableFamiliesData(data))
        end
    catch
        return []
    end
end

# () -> [FunctionName, nargs, ]
function sendParameterData()
    getLastCallParameters()
end

function sendGlobalVariables(b::Unity)
    try
    data = []
    foreach(x->append!(data, x), getGlobalVariables())
    print("DATA: ", data)
    @remote(b, StoreGlobalVariables(data))
    catch e
        showerror(stdout,e)
    end
end

function getGlobalVariables(apply_filter=true)
    vars = Khepri.varinfo()
    foreach(x -> append!(x, [isnothing(CodeTools.getthing(x[1])) ? "" : (occursin("Parameter", string(CodeTools.getthing(x[1]))) ? match(r"Parameter{.*}\((?<val>.*)\)",string(CodeTools.getthing(x[1])))[:val] : string(CodeTools.getthing(x[1])))]), vars)
    filter(x -> apply_filter ? occursin("Int", x[2]) || occursin("Float", x[2]) : !isnothing(x), vars)
end

function getExistingFamilies(type)

end

# Adapted from InteractiveUtils
function varinfo()
    m = CodeTools.getthing("Main")
    rows =
        Any[ let value = CodeTools.getthing(string(v))
                 Any[string(v), summary(value)]
             end
             for v in sort!(names(m)) if isdefined(m, v)]
    return rows
end

function locateVariable(name, file)
    fileToRead = open(file)
    for i in enumerate(eachline(fileToRead))
        try
            current = Meta.parse(i[2])
            if (!isnothing(current) && current.head == :(=) && current.args[1] == Symbol(name))
                close(fileToRead)
                return i[1]
            end
        catch x end
    end
    close(fileToRead)
end


function getLastCallParameters()
    call = readLine(string(fileAndLinesOfSelected[1][end][1]),fileAndLinesOfSelected[1][end][2])
    call_name = string(Meta.parse(call).args[1])
    max_count = 0
    largest_method = nothing
    print(typeof(call_name))
    #for m in methods_new(getfield(Main, call_name), Main)
    for m in methods(CodeTools.getthing(call_name))
        if (m.nargs > max_count)
            largest_method = m
            max_count = m.nargs
        end
    end
    Base.arg_decl_parts(largest_method)[2][2:end]
end

function getParametersValue()
    # TODO: get value in runtime
    # TODO: default/optional values
    call = readLine(string(fileAndLinesOfSelected[1][end][1]),fileAndLinesOfSelected[1][end][2])
    expr = Meta.parse(call).args[2:end]
end

function getLastCallInfo()
    # TODO: eval values
    # TODO: inspect optional/default values

end

function openAddPanel(b::Unity)
        @remote(b, OpenAddPanel())
end

################################################################################
##                          MANIPULATION FUNCTIONS                            ##
################################################################################
# Functions to be called by Unity that apply changes to code
# TODO: Make pretty
# TODO: Fix multiple applications

function updateFamilyValues(args...)
    #TODO: make pretty
    #TODO: switch
    try
        args = args[1]
        print("--> Updated data: ", args,"\n")
        params = []
        for i = 2:2:length(args)-1
            if (args[i-1] == "material")
                changeMaterial(typeof(selected.family), args[end], args[i])
                return
            end
            if (args[i-1] == "profile")
                append!(params, [(Symbol(args[i-1]), Meta.parse(args[i]))])
            elseif (iseven(i))
                append!(params, [(Symbol(args[i-1]), parse(Float64, args[i]))])
            end
        end
        changeFamily(typeof(selected.family), args[end], params...)
    catch e
        showerror(stdout,e)
    end
end

function updateGlobalVariables(args...)
    #source_file = raw"D:\Catarina\OneDrive - Universidade de Lisboa\IST\Tese\Code\simple.jl"
    try
    args = args[1]
    for i = 2:2:length(args)
        changeGlobalVariable(args[i-1],args[i])
    end
catch e
    showerror(stdout,e)
end
end

function changeGlobalVariable(name, new_value)
    #source_file = raw"D:\Catarina\OneDrive - Universidade de Lisboa\IST\Tese\Code\simple.jl"
    line = locateVariable(name, source_file)
    original = readLine(source_file, line)
    command = toVar(Meta.parse(original), parse(Float64, new_value))
    applyChange(Change("Variable change: ", source_file, line, 1, original, command))
end

familySymbols=Dict( SlabFamily  => :slab_family,
                    WallFamily  => :wall_family,
                    PanelFamily => :panel_family,
                    ColumnFamily => :column_family,
                    BeamFamily => :beam_family)



function toCall(f, args...)
    x = [isa(arg,Tuple) ? :($(arg[1])=$(arg[2])) : arg for arg in args]
    :($(f)($(x...)))
end

macro wrap(out, in)
    #TODO: expr
end

function wrap2(out, in)
    string("$out do\n$in\nend")
end

function toVar(expr, value)
    Expr(:(=), expr.args[1], Expr(:call, :+, expr.args[2], value))
end

function changeFamily(type, func, args...)
    functionToUpdate = callChainDict[func]
    out = toCall(Symbol(:with_, familySymbols[type]), args...)
    line = readLine(functionToUpdate[1], functionToUpdate[2])
    command = wrap2(out, line)
    applyChange(Change(type, functionToUpdate[1], functionToUpdate[2], 3, line, command))
end

function changeMaterial(type, func, path)
    functionToUpdate = callChainDict[func]
    name = familySymbols[type]
    var = MacroTools.gensym_ids(gensym("$type"))
    out = "$var = $(toCall(Symbol(name,:_element), toCall(Symbol(:default_,name))))\n$(toCall(:set_backend_family, var, :unity, toCall(:unity_material_family, path)))\n$(toCall(:with, Symbol(:default_,name), var))"
    println(out)
    line = readLine(functionToUpdate[1], functionToUpdate[2])
    command = wrap2(out, line)
    println(command)
    applyChange(Change(type, functionToUpdate[1], functionToUpdate[2], 5, line, command))
end

function extractFamily(line)
    a = Meta.parse(line)
end

#TODO: truss node and bar
shapeSymbs = Dict(  Door => :door, Window => :window,
                    Pointlight => :pointlight, TableAndChairs => :table_and_chairs,
                    Table => :table, Chair => :chair,
                    Wall => :wall, Roof => :roof, Panel => :panel, CurtainWall => :curtain_wall,
                    Slab => :slab, Column => :column, FreeColumn => :free_column)

function createObject(type, args...)
    if (type == Wall || type == Door)
        print("no can do")
    end
    command = toCall(shapeSymbs[type], args...)
    print(command)
    #TODO: add principal file
    #applyChange(Change(type, _, _, 1, "", command))
end

function changeObject(type, args...)

end


#TODO: Add to reverseHighlight?
# Extract function names from call chain: Array{String}
function extractCallChainNames(saveInDict=false)
    if (!saveInDict)
        names = filter(!isempty, map(x -> addToDict(x), fileAndLinesOfSelected[1]))
        #string(extractFirstCall(Meta.parse(readLine(string(x[1]), x[2])))),

    else
        foreach(x -> callChainDict[string(extractFirstCall(Meta.parse(readLine(string(x[1]), x[2]))))] = x,
        fileAndLinesOfSelected[1])
    end
end

function addToDict(x)
    name = string(extractFirstCall(Meta.parse(readLine(string(x[1]), x[2]))))
    global callChainDict[name] = x
    name
end

#####
## AST manipulation
#####

#TODO: make it work for expr in args beside 2, with calls
# Extract the first call name of a give Expr: Symbol
function extractFirstCall(expr::Expr)
    try
        if (expr.head == :call)
            return expr.args[1]
        elseif (isa(expr.args[2],Expr))
            return extractFirstCall(expr.args[2])
        else
            return ""
    end
    catch e
          return ""
    end
end
################################################################################
##                       FILE MANIPULATION FUNCTIONS                          ##
################################################################################
# Functions that deal with the file's manipulation (read, write)
# TODO: Add Atom packages function to live manipulation

function saveChanges(file)
    if (file == "none")
        file = test
    end
    file_edit = string(file[1:end-3], "_edit.jl")
    println(file_edit, "\n")
    original = open(file)
    edited = open(file_edit, "w")
    for i in enumerate(eachline(original))
        # i[1]: line number
        # i[2]: line content
        println(i[1], ": ", i[2])
        if (haskey(changes, i[1]))
            write(edited, "$(changes[i[1]])\n")
        else
            write(edited, "$(i[2])\n")
        end
    end
    close(edited)
    close(original)
end
#TODO: fix none
function readLine(file, line)
    if (isa(file, Symbol))
        file = string(file)
    end
    if (file == "none")
        file = test
    end
    if (newFile)
        file_edit = string(file[1:end-3], "_edit.jl")
        if (isfile(file_edit))
            file = file_edit
        end
    end
    fileToRead = open(file)
    for i in enumerate(eachline(fileToRead))
        # i[1]: line number
        # i[2]: line content
        if (i[1] == line)
            close(fileToRead)
            return i[2]
        end
    end
end

function writeLine(file, command)
    if (file == "none")
        file = test
    end
    fileToWrite = open(string(file), "a")
    write(fileToWrite, command)
    close(fileToWrite)
end


# Substitute line
function writeLine(file, line, command)
    if (isa(file, Symbol))
        file = string(file)
    end
    if (file == "none")
        file = test
    end
    if (newFile)
        file_edit = string(file[1:end-3], "_edit.jl")
        if (isfile(file_edit))
            file = file_edit
        end
    end
    fileToRead = open(file)
    fileToWrite = open(string(file[1:end-3], "_temp.jl"), "w")
    for i in enumerate(eachline(fileToRead))
        if (i[1] == line)
            write(fileToWrite, "$(command)\n")
        else
            write(fileToWrite, "$(i[2])\n")
        end
    end
    close(fileToRead)
    close(fileToWrite)
    mv(string(file[1:end-3], "_temp.jl"), file, force=true)
end

function runFile(file)
    include(isa(file,String) ? file : string(file))
    if (state == 3)
        clear_trace!()
        trace!(selected)
        global fileAndLinesOfSelected = reverseHighlight(selected)
    end
end

################################################################################
##                          SCRIPT-IT FUNCTIONS                               ##
################################################################################
# Temporary solution to select_shape without package conflicts
# TODO: Fix package conflicts
isControlFlow = true

function reverseHighlight(shape)
    highlight_shape(shape)
    actualCodeLinesHighlighted = []
    fileAndLinesToHighlight = Dict()
    callChainFunctions = Dict()
    stringRanges = ""
    refValVal = shape
    for dShape in keys(Khepri.shape_to_file_locations)
        dictShape = dShape
        if(refValVal == dictShape && length(Khepri.shape_to_file_locations[dShape])>0)
            if(isControlFlow)
                actualCodeLinesHighlighted = vcat(actualCodeLinesHighlighted,[Khepri.shape_to_file_locations[dShape]])
            else
                actualCodeLinesHighlighted = vcat(actualCodeLinesHighlighted,first(Khepri.shape_to_file_locations[dShape]))
            end
        end
    end
    actualCodeLinesHighlighted = unique(actualCodeLinesHighlighted)
    #TODO: fix occasionally bounds error here
    #print("here ", actualCodeLinesHighlighted[1])
    #foreach(x -> callChainFunctions[string(extractFirstCall(Meta.parse(readLine(x[1], x[2]))))] = (x[1], x[2]), actualCodeLinesHighlighted[1])
    return actualCodeLinesHighlighted
end

function convertToDictAndToList(list)
    tempDict=Dict()
    file = nothing
    for el in list
        lineJoin = []
        if (isa(el, Tuple)) #This if is necessary to allow the reverseHighlight of last function call mode
            file = string(el[1])
            line = el[2]
            lineJoin = vcat(lineJoin, line)
        else
            for lineIter in el
                file = string(lineIter[1])
                line = lineIter[2]
                lineJoin = vcat(lineJoin, line)
            end
        end
        if(haskey(tempDict,file))
            tempDict[file] = vcat(tempDict[file],[lineJoin])
        else
            tempDict[file] = [lineJoin]
        end
    end
    finalList = []
    for key in keys(tempDict)
        finalList = vcat(finalList, [[key, unique(tempDict[key])]])
    end
    return finalList
end
