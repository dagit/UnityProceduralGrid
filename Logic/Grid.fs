namespace Logic
open UnityEngine
open System.Collections

[<RequireComponent(typeof<MeshFilter>, typeof<MeshRenderer>)>]
type public Grid() =
  inherit MonoBehaviour()

  let mutable verticies : Vector3 array = [||]

  [<SerializeField>]
  let mutable xSize : int = Unchecked.defaultof<int>
  [<SerializeField>]
  let mutable ySize : int = Unchecked.defaultof<int>

  let mutable mesh : Mesh = Unchecked.defaultof<Mesh>

  member __.Generate() =
    let wait : WaitForSeconds = new WaitForSeconds(0.05f);

    mesh <- new Mesh()
    __.GetComponent<MeshFilter>().mesh <- mesh
    mesh.name <- "Procedural Grid"


    verticies    <- Array.zeroCreate ((xSize + 1) * (ySize + 1))
    let uv       =  Array.zeroCreate (verticies.Length)
    let tangents =  Array.zeroCreate (verticies.Length)
    let tangent  =  new Vector4(1.0f, 0.0f, 0.0f, -1.0f)

    let mutable i = 0
    for y in seq { 0 .. ySize } do
      for x in seq { 0 .. xSize } do
        verticies.[i] <- new Vector3(float32 x, float32 y)
        uv.[i]        <- new Vector2(float32 x / float32 xSize, float32 y / float32 ySize)
        tangents.[i]  <- tangent
        i <- i + 1
    mesh.vertices <- verticies
    mesh.uv       <- uv
    mesh.tangents <- tangents
    let mutable triangles = Array.zeroCreate (xSize * ySize * 6)
    let mutable vi = 0
    let mutable ti = 0
    for y in 0 .. ySize - 1 do
      for x in 0 .. xSize - 1 do
        triangles.[ti + 0] <- vi
        triangles.[ti + 1] <- vi + xSize + 1
        triangles.[ti + 2] <- vi + 1
        triangles.[ti + 3] <- vi + 1
        triangles.[ti + 4] <- vi + xSize + 1
        triangles.[ti + 5] <- vi + xSize + 2
        mesh.triangles <- triangles
        mesh.RecalculateNormals()
        ti <- ti + 6
        vi <- vi + 1
      vi <- vi + 1

  member __.Awake() =
    __.Generate()

  member __.OnDrawGizmos() =
    Gizmos.color <- Color.black
    for v in verticies do
      Gizmos.DrawSphere(__.transform.TransformPoint(v), 0.1f)
