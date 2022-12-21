module PlatBerryHiddenStrawberry

using ..Ahorn, Maple

@mapdef Entity "PlatinumStrawberry/HiddenStrawberry" HiddenStrawberry(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Hidden Strawberry" => Ahorn.EntityPlacement(
        HiddenStrawberry
    )
)

const sprite = "collectables/ghostberry/idle00.png"

function Ahorn.selection(entity::HiddenStrawberry)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::HiddenStrawberry, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end