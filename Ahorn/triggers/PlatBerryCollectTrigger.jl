module PlatBerryPlatBerryCollectTrigger

using ..Ahorn, Maple

@mapdef Trigger "PlatinumStrawberry/PlatBerryCollectTrigger" PlatBerryCollectTrigger(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight)

const placements = Ahorn.PlacementDict(
    "Platinum Berry Collection" => Ahorn.EntityPlacement(
        PlatBerryCollectTrigger,
        "rectangle",
    ),
)

end