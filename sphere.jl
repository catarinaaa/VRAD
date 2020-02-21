using Khepri
backend(unity)
Khepri.collected_shapes(Khepri.Shape[])
Khepri.in_shape_collection(true)

function create_spheres(radius)
    sphere(xyz(0, 3, 0), radius)
    sphere(xyz(0, 6, 0), radius)
end
sphere(xyz(0, 0, 0))

create_spheres(1)
VRediting()
