module PlatBerryPlatinumStrawberry

using ..Ahorn, Maple

@mapdef Entity "PlatinumStrawberry/PlatinumStrawberry" PlatinumStrawberry(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Platinum Strawberry" => Ahorn.EntityPlacement(
        PlatinumStrawberry
    )
)

const sprite = "SyrenyxPlatinumStrawberry/collectables/platinumberry/idle00.png"

function Ahorn.selection(entity::PlatinumStrawberry)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::PlatinumStrawberry, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end