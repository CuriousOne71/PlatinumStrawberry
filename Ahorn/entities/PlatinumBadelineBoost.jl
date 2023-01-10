module PlatBerryPlatinumBadelineBoost

using ..Ahorn, Maple

@mapdef Entity "PlatinumStrawberry/PlatinumBadelineBoost" PlatinumBadelineBoost(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Platinum Badeline Boost" => Ahorn.EntityPlacement(
        PlatinumBadelineBoost
    )
)

const sprite = "objects/badelineboost/idle00.png"

function Ahorn.selection(entity::PlatinumBadelineBoost)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::PlatinumBadelineBoost, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end