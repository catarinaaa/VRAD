### File with all the VRAD related functions ###
export VRediting
#States
#0.Deactivated
#1.Main
#2.InSelection
#3.Selected

#Variables
active = false
state = 0

#Functions



#myfunction is a test function to start cycle
@defop myfunction(prompt::String="nothing")
@defop queryUnity(prompt::String="nothing")
@defop startSelected(prompt::String="nothing")
@defop VRediting(prompt::String="nothing")

function VRediting(prompt::String, b::Unity)
    print("***  VRAD is active   ***")
    active = true
    state = 1
    @remote(b, EnableVRAD())
end

function myfunction(prompt::String, b::Unity)
    @async queryUnity()
end

function queryUnity(prompt::String, b::Unity)
     while true
         sleep(0.1)
         let s = @remote(b, CallJulia())
         if (s != "empty")
             runFunction(s)
         end
     end
   end
 end

function runFunction(name)
    getfield(Khepri, Symbol(name))()
end

 function startInSelection()
     state = 2
     print("*** Editing mode is on  ***\n")
     global selected = select_shape()
     startSelected()

 end

 function startSelected(prompt::String, b::Unity)
     state = 3
     print("*** Object is selected  ***")
     #openoptions
     @remote(b, ChangeState(2))
 end

function stopEditingMode()
    #save state?
    state = 1
    print("***  Editing mode is off ***")
end

function getShapeInfo()
    print(selected)
    #@remote(b, selected)
end
